namespace Morgemil.Core

type IRow =
    abstract member Key : int64

type IIndex<'tRow when 'tRow :> IRow> =
    abstract member AddRow: 'tRow -> unit
    abstract member UpdateRow: 'tRow -> 'tRow -> unit
    abstract member Remove: 'tRow -> unit

type IUniqueIndex<'tRow, 'tKey when 'tRow :> IRow> =
    abstract member RemoveByKey: 'tKey -> unit
    abstract member Item: 'tKey -> 'tRow with get, set
    abstract member TryGetRow: 'tKey -> 'tRow option
        
type IMultiIndex<'tRow, 'tKey when 'tRow :> IRow> =
    abstract member Item: 'tKey -> 'tRow list with get
    abstract member TryGetRows: 'tKey -> 'tRow list

type IPrimaryIndex<'tRow, 'tKey when 'tRow :> IRow> =
    inherit IIndex<'tRow>
    abstract member RemoveByKey: 'tKey -> unit
    abstract member Items: 'tRow seq
    abstract member Item: 'tKey -> 'tRow with get, set
    abstract member TryGetRow: 'tKey -> 'tRow option
    
type IReadonlyTable<'tRow when 'tRow :> IRow> =
    abstract member TryGetRow: int64 -> 'tRow option    
    abstract member Items: 'tRow seq
    abstract member Item: int64 -> 'tRow with get, set

type ITable<'tRow when 'tRow :> IRow> =
    inherit IReadonlyTable<'tRow>
    abstract member AddRow: 'tRow -> unit
    abstract member UpdateRow: 'tRow -> 'tRow -> unit
    abstract member Remove: 'tRow -> unit
    abstract member RemoveByKey: int64 -> unit
    abstract member GenerateKey: unit -> int64

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
            and set key row = _dictionary.[ key ] <- row
        member this.RemoveByKey key = _dictionary.Remove key |> ignore
        
    interface IIndex<'tRow> with
        member this.Remove row = _dictionary.Remove (getKey row) |> ignore
        member this.AddRow row = _dictionary.[ getKey row ] <- row
        member this.UpdateRow oldRow row =
            let oldKey = getKey oldRow
            let newKey = getKey row
            if oldKey <> newKey then
                _dictionary.Remove oldKey |> ignore
            _dictionary.[ newKey ] <- row


type MultiIndex<'tRow, 'tKey when 'tKey: equality and 'tRow :> IRow>(getKey: 'tRow -> 'tKey) =
    let _dictionary = new System.Collections.Generic.Dictionary<'tKey, 'tRow list>()
            
    interface IIndex<'tRow> with
        member this.Remove row =
            let key = getKey row
            match _dictionary.TryGetValue key with
            | true, value ->
                if value.Length = 1 then
                    _dictionary.Remove key |> ignore
                else
                    _dictionary.[key] <- value |> List.filter(fun t -> t.Key <> row.Key)
            | _ -> ()
        member this.AddRow row =
            let key = getKey row            
            match _dictionary.TryGetValue key with
            | true, value -> _dictionary.[key] <- row :: value
            | _ -> _dictionary.[key] <- [ row ]
        member this.UpdateRow oldRow row =
            let oldKey = getKey oldRow
            let newKey = getKey row
            if oldKey = newKey then
                _dictionary.[newKey] <- row :: (_dictionary.[newKey] |> List.filter(fun t -> t.Key <> row.Key))
            else 
                _dictionary.[oldKey] <- (_dictionary.[oldKey] |> List.filter(fun t -> t.Key <> row.Key))
                match _dictionary.TryGetValue newKey with
                | true, value -> _dictionary.[newKey] <- row :: (value |> List.filter(fun t -> t.Key <> row.Key))
                | _ -> _dictionary.[newKey] <- [ row ]

    interface IMultiIndex<'tRow, 'tKey> with
        member this.TryGetRows(key: 'tKey): 'tRow list = 
            match _dictionary.TryGetValue key with
            | true, value -> value
            | _ ->List.empty
        member this.Item
            with get key =
                match _dictionary.TryGetValue key with
                | true, value -> value
                | _ -> List.empty


type PrimaryIndex<'tRow when 'tRow :> IRow>() =
    let _dictionary = new System.Collections.Generic.Dictionary<int64, 'tRow>()
            
    interface IPrimaryIndex<'tRow, int64> with
        member this.Items = _dictionary.Values |> Seq.map id
        member this.Remove row = _dictionary.Remove row.Key |> ignore
        member this.RemoveByKey key = _dictionary.Remove key |> ignore
        member this.AddRow row = _dictionary.[ row.Key ] <- row
        member this.TryGetRow key = 
            match _dictionary.TryGetValue key with
            | true, value -> Some value
            | _ -> None
        member this.Item
            with get key =
                match _dictionary.TryGetValue key with
                | true, value -> value
                | _ -> failwithf "Couldn't find key %A" key
            and set key row = _dictionary.[ key ] <- row            
        member this.UpdateRow oldRow row =
            let oldKey = oldRow.Key
            let newKey = row.Key
            if oldKey <> newKey then
                _dictionary.Remove oldKey |> ignore
            _dictionary.[ newKey ] <- row

type Table<'tRow when 'tRow :> IRow>() =
    let _primaryKey = new PrimaryIndex<'tRow>() :> IPrimaryIndex<'tRow, int64>
    let _primaryKeyIndexCast = _primaryKey :> IIndex<'tRow>
    let mutable _indices = [ _primaryKeyIndexCast ]
    let mutable _nextKey = 0L

    let _setNextKey otherKey = _nextKey <- System.Math.Max(otherKey + 1L, _nextKey)

    member this.AddIndex index = _indices <- index :: _indices
    member this.PrimaryIndex = _primaryKeyIndexCast
    
    interface ITable<'tRow> with
        member this.GenerateKey() =
            let key = _nextKey
            _nextKey <- _nextKey + 1L
            key
        member this.Item
            with get key = _primaryKey.[ key ]
            and set key row = (this :> ITable<'tRow>).AddRow row
        member this.Items = _primaryKey.Items
        member this.Remove row = _indices |> List.iter(fun t -> t.Remove row)
        member this.RemoveByKey key =
            match _primaryKey.TryGetRow key with 
            | Some(row) -> _indices |> List.iter(fun t -> t.Remove row)
            | None -> ()
        member this.AddRow (row: 'tRow) =            
            match _primaryKey.TryGetRow (row :> IRow).Key with
            | Some(oldRow) -> 
                _indices |> List.iter(fun t -> t.UpdateRow oldRow row)
            | None -> 
                _setNextKey (row :> IRow).Key
                _indices |> List.iter(fun t -> t.AddRow row)
        member this.TryGetRow key = _primaryKey.TryGetRow key
        member this.UpdateRow oldRow row =         
            match _primaryKey.TryGetRow (row :> IRow).Key with
            | Some(oldRow) -> 
                _indices |> List.iter(fun t -> t.UpdateRow oldRow row)
            | None -> ()

module Table =
    let Items (table: 'T when 'T :> IReadonlyTable<'U>) =
        table.Items

    let AddRow (table: 'T when 'T :> ITable<'U>) (row: 'U) =
        table.AddRow(row)

    let GetRowByKey (table: 'T when 'T :> IReadonlyTable<_>) key =
        table.Item key

    let TryGetRowByKey (table: 'T when 'T :> IReadonlyTable<_>) key =
        table.TryGetRow key

    let RemoveRow (table: 'T when 'T :> ITable<'U>) (row: 'U) =
        table.Remove(row)

    let RemoveRowByKey (table: 'T when 'T :> ITable<_>) key =
        table.RemoveByKey key

    let GenerateKey (table: 'T when 'T :> ITable<_>) =
        table.GenerateKey()

    let CreateReadonlyTable (rows: seq<'T> when 'T :> IRow): IReadonlyTable<'T> =
        let table = new Table<'T>()
        rows |> Seq.iter(AddRow table)
        table :> IReadonlyTable<'T>

module MultiIndex =
    let GetRowsByKey (index: IMultiIndex<_,'T>) (key: 'T) =
        (index :> IMultiIndex<_,_>).TryGetRows key
