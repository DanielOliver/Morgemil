namespace Morgemil.Core

open Morgemil.Models.Relational
open Morgemil.Models


type ITrackedHistory =
    abstract member HistoryCallback: (StepItem -> unit) with get, set

type UniqueIndex<'tRow, 'tKey when 'tKey: equality and 'tRow :> IRow>(getKey: 'tRow -> 'tKey) =
    let _dictionary = new System.Collections.Generic.Dictionary<'tKey, 'tRow>()

    interface IUniqueIndex<'tRow, 'tKey> with
        member this.TryGetRow key =
            match _dictionary.TryGetValue key with
            | true, value -> Some value
            | _ -> None

        member this.Item
            with get key =
                match _dictionary.TryGetValue key with
                | true, value -> value
                | _ -> failwithf "Couldn't find key %A" key
            and set key row = _dictionary.[key] <- row

        member this.RemoveByKey key = _dictionary.Remove key |> ignore

    interface IIndex<'tRow> with
        member this.Remove row =
            _dictionary.Remove(getKey row) |> ignore

        member this.Add row = _dictionary.[getKey row] <- row

        member this.Update oldRow row =
            let oldKey = getKey oldRow
            let newKey = getKey row

            if oldKey <> newKey then
                _dictionary.Remove oldKey |> ignore

            _dictionary.[newKey] <- row


type MultiIndex<'tRow, 'tKey when 'tKey: equality and 'tRow :> IRow>(getKey: 'tRow -> 'tKey) =
    let _dictionary = System.Collections.Generic.Dictionary<'tKey, 'tRow list>()

    interface IIndex<'tRow> with
        member this.Remove row =
            let key = getKey row

            match _dictionary.TryGetValue key with
            | true, value ->
                if value.Length = 1 then
                    _dictionary.Remove key |> ignore
                else
                    _dictionary.[key] <- value |> List.filter (fun t -> t.Key <> row.Key)
            | _ -> ()

        member this.Add row =
            let key = getKey row

            match _dictionary.TryGetValue key with
            | true, value -> _dictionary.[key] <- row :: value
            | _ -> _dictionary.[key] <- [ row ]

        member this.Update oldRow row =
            let oldKey = getKey oldRow
            let newKey = getKey row

            if oldKey = newKey then
                _dictionary.[newKey] <- row :: (_dictionary.[newKey] |> List.filter (fun t -> t.Key <> row.Key))
            else
                _dictionary.[oldKey] <- (_dictionary.[oldKey] |> List.filter (fun t -> t.Key <> row.Key))

                match _dictionary.TryGetValue newKey with
                | true, value -> _dictionary.[newKey] <- row :: (value |> List.filter (fun t -> t.Key <> row.Key))
                | _ -> _dictionary.[newKey] <- [ row ]

    interface IMultiIndex<'tRow, 'tKey> with
        member this.TryGetRows(key: 'tKey) : 'tRow seq =
            match _dictionary.TryGetValue key with
            | true, value -> value |> seq
            | _ -> Seq.empty

        member this.Item
            with get key =
                match _dictionary.TryGetValue key with
                | true, value -> value |> seq
                | _ -> Seq.empty


type PrimaryIndex<'tRow when 'tRow :> IRow>() =
    let _dictionary = new System.Collections.Generic.Dictionary<int64, 'tRow>()

    interface IPrimaryIndex<'tRow, int64> with
        member this.Items = _dictionary.Values |> Seq.map id
        member this.Remove row = _dictionary.Remove row.Key |> ignore
        member this.RemoveByKey key = _dictionary.Remove key |> ignore
        member this.Add row = _dictionary.[row.Key] <- row

        member this.TryGetRow key =
            match _dictionary.TryGetValue key with
            | true, value -> Some value
            | _ -> None

        member this.Item
            with get key =
                match _dictionary.TryGetValue key with
                | true, value -> value
                | _ -> failwithf "Couldn't find key %A" key
            and set key row = _dictionary.[key] <- row

        member this.Update oldRow row =
            let oldKey = oldRow.Key
            let newKey = row.Key

            if oldKey <> newKey then
                _dictionary.Remove oldKey |> ignore

            _dictionary.[newKey] <- row

type KeyGeneration<'tKey>(toKey: int64 -> 'tKey) =
    let mutable _nextKey = 0L

    let _setNextKey otherKey =
        _nextKey <- System.Math.Max(otherKey + 1L, _nextKey)

    interface IGenerateKeys<'tKey> with
        member this.CheckKey key = key |> _setNextKey

        member this.GenerateKey() =
            let key = _nextKey
            _nextKey <- _nextKey + 1L
            key |> toKey

type Table<'tRow, 'tKey when 'tRow :> IRow>
    (fromKey: 'tKey -> int64, generator: IGenerateKeys<'tKey>, historyIdentity: TableEvent<'tRow> -> StepItem) =
    let _primaryKey = new PrimaryIndex<'tRow>() :> IPrimaryIndex<'tRow, int64>

    let _primaryKeyIndexCast = _primaryKey :> IIndex<'tRow>
    let mutable _indices = [ _primaryKeyIndexCast ]
    let mutable _trackedRecordEvent = ignore

    new(toKey, fromKey, history) = Table(fromKey, KeyGeneration(toKey), history)

    member this.AddIndex index = _indices <- index :: _indices
    member this.PrimaryIndex = _primaryKeyIndexCast

    interface ITrackedHistory with
        member this.HistoryCallback
            with get () = _trackedRecordEvent
            and set value = _trackedRecordEvent <- value

    interface IReadonlyTable<'tRow, 'tKey> with
        member this.Item
            with get key = _primaryKey.[fromKey key]

        member this.Items = _primaryKey.Items
        member this.TryGetRow key = key |> fromKey |> _primaryKey.TryGetRow

    interface ITable<'tRow, 'tKey> with
        member this.GenerateKey() = generator.GenerateKey()

        member this.Item
            with get key = _primaryKey.[key |> fromKey]
            and set _ row = (this :> ITable<'tRow, 'tKey>).Add row

        member this.Remove row =
            _indices |> List.iter (fun t -> t.Remove row)
            (TableEvent.Removed row) |> historyIdentity |> _trackedRecordEvent

        member this.RemoveByKey key =
            match key |> fromKey |> _primaryKey.TryGetRow with
            | Some row -> (this :> ITable<'tRow, 'tKey>).Remove row
            | None -> ()

        member this.Add(row: 'tRow) =
            let key = (row :> IRow).Key

            match _primaryKey.TryGetRow key with
            | Some oldRow ->
                _indices |> List.iter (fun t -> t.Update oldRow row)
                TableEvent.Updated(oldRow, row) |> historyIdentity |> _trackedRecordEvent
            | None ->
                generator.CheckKey key
                _indices |> List.iter (fun t -> t.Add row)
                TableEvent.Added row |> historyIdentity |> _trackedRecordEvent

        member this.Update _ row =
            match _primaryKey.TryGetRow (row :> IRow).Key with
            | Some oldRow ->
                _indices |> List.iter (fun t -> t.Update oldRow row)
                TableEvent.Updated(oldRow, row) |> historyIdentity |> _trackedRecordEvent
            | None -> ()

type ReadonlyTable<'tRow, 'tKey when 'tRow :> IRow>(rows: seq<'tRow>, fromKey: ^tKey -> int64) =
    let _rows: 'tRow[] = rows |> Seq.toArray

    let _primaryKeys: int64[] = rows |> Seq.map (fun t -> t.Key) |> Seq.toArray

    do System.Array.Sort(_primaryKeys, _rows)

    interface IReadonlyTable<'tRow, 'tKey> with
        member this.Item
            with get key =
                let intKey = fromKey key

                let index = System.Array.BinarySearch(_primaryKeys, intKey)

                if index < 0 then
                    failwithf "Can't find key %i" intKey

                _rows.[index]

        member this.Items = _rows |> Seq.cache

        member this.TryGetRow key =
            let index = System.Array.BinarySearch(_primaryKeys, fromKey key)

            if index < 0 then None else _rows.[index] |> Some

module Table =
    let Items (table: 'T :> IReadonlyTable<'U, _>) : 'U seq = table.Items

    let AddRow (table: 'T :> ITable<'U, _>) (row: 'U) : unit = table.Add(row)

    let GetRowByKey (table: 'T :> IReadonlyTable<'Row, 'U>) (key: 'U) : 'Row = table.Item key

    let TryGetRowByKey (table: 'T :> IReadonlyTable<'Row, 'U>) (key: 'U) : 'Row option = table.TryGetRow key

    let RemoveRow (table: 'T :> ITable<'U, _>) (row: 'U) : unit = table.Remove(row)

    let RemoveRowByKey (table: 'T :> ITable<_, 'U>) (key: 'U) : unit = table.RemoveByKey key

    let GenerateKey (table: 'T :> ITable<_, 'U>) : 'U = table.GenerateKey()

    let CreateReadonlyTable (fromKey: 'U -> int64) (rows: seq<'T> when 'T :> IRow) : IReadonlyTable<'T, 'U> =
        let table = new ReadonlyTable<'T, 'U>(rows, fromKey)
        table :> IReadonlyTable<'T, 'U>

    let EmptyScenarioData: ScenarioData =
        { Items = CreateReadonlyTable (fun (ItemID id) -> id) []
          Ancestries = CreateReadonlyTable (fun (AncestryID id) -> id) []
          Tiles = CreateReadonlyTable (fun (TileID id) -> id) []
          TileFeatures = CreateReadonlyTable (fun (TileFeatureID id) -> id) []
          Heritages = CreateReadonlyTable (fun (HeritageID id) -> id) []
          FloorGenerationParameters = CreateReadonlyTable (fun (FloorGenerationParameterID id) -> id) []
          MonsterGenerationParameters = CreateReadonlyTable (fun (MonsterGenerationParameterID id) -> id) []
          Aspects = CreateReadonlyTable (fun (AspectID id) -> id) []
          Towers = CreateReadonlyTable (fun (TowerID id) -> id) [] }

module TableQuery =
    let SeqLeftJoin
        (left: seq<'T>)
        (getForeignKeyLeft: 'T -> 'W)
        (right: IReadonlyTable<'U, 'W>)
        : seq<'T * 'U option> =
        left
        |> Seq.map (fun leftRow ->
            let rightRow = leftRow |> getForeignKeyLeft |> right.TryGetRow

            leftRow, rightRow)

    let LeftJoin
        (left: IReadonlyTable<'T, _>)
        (getForeignKeyLeft: 'T -> 'X)
        (right: IReadonlyTable<'U, 'X>)
        : seq<'T * 'U option> =
        left.Items
        |> Seq.map (fun leftRow ->
            let rightRow = leftRow |> getForeignKeyLeft |> right.TryGetRow

            leftRow, rightRow)

    let SeqInnerJoin (left: seq<'T>) (getForeignKeyLeft: 'T -> 'W) (right: IReadonlyTable<'U, 'W>) : seq<'T * 'U> =
        SeqLeftJoin left getForeignKeyLeft right
        |> Seq.choose (fun (leftRow, rightRowOption) ->
            match rightRowOption with
            | Some rightRow -> Some(leftRow, rightRow)
            | None -> None)

    let InnerJoin
        (left: IReadonlyTable<'T, _>)
        (getForeignKeyLeft: 'T -> 'W)
        (right: IReadonlyTable<'U, 'W>)
        : seq<'T * 'U> =
        LeftJoin left getForeignKeyLeft right
        |> Seq.choose (fun (leftRow, rightRowOption) ->
            match rightRowOption with
            | Some rightRow -> Some(leftRow, rightRow)
            | None -> None)

module MultiIndex =
    let GetRowsByKey (index: IMultiIndex<'W, 'T>) (key: 'T) : 'W seq = index.TryGetRows key

    let Item (index: IMultiIndex<_, 'T>) (key: 'T) = index.[key]
