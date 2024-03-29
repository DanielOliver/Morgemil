namespace Morgemil.Models.Relational

type IRow =
    abstract member Key: int64

[<RequireQualifiedAccess>]
type TableEventType =
    | Added
    | Updated
    | Removed

type TableEvent<'tRow when 'tRow :> IRow> =
    | Added of newValue: 'tRow
    | Updated of oldValue: 'tRow * newValue: 'tRow
    | Removed of oldValue: 'tRow

    member this.TableEventType =
        match this with
        | Added _ -> TableEventType.Added
        | Removed _ -> TableEventType.Removed
        | Updated _ -> TableEventType.Updated

    member this.Map(mapper: _ -> _) =
        match this with
        | Added x -> x |> mapper |> Added
        | Removed x -> x |> mapper |> Removed
        | Updated(x, y) -> (x |> mapper, y |> mapper) |> Updated

type IGenerateKeys<'tKey> =
    abstract member GenerateKey: unit -> 'tKey
    abstract member CheckKey: int64 -> unit

type IIndex<'tRow when 'tRow :> IRow> =
    abstract member Add: next: 'tRow -> unit
    abstract member Update: old: 'tRow -> next: 'tRow -> unit
    abstract member Remove: old: 'tRow -> unit

type IUniqueIndex<'tRow, 'tKey when 'tRow :> IRow> =
    abstract member RemoveByKey: 'tKey -> unit
    abstract member Item: 'tKey -> 'tRow with get, set
    abstract member TryGetRow: 'tKey -> 'tRow option

type IMultiIndex<'tRow, 'tKey when 'tRow :> IRow> =
    abstract member Item: 'tKey -> 'tRow seq with get
    abstract member TryGetRows: 'tKey -> 'tRow seq

type IPrimaryIndex<'tRow, 'tKey when 'tRow :> IRow> =
    inherit IIndex<'tRow>
    abstract member RemoveByKey: 'tKey -> unit
    abstract member Items: 'tRow seq
    abstract member Item: 'tKey -> 'tRow with get, set
    abstract member TryGetRow: 'tKey -> 'tRow option

type IReadonlyTable<'tRow, 'tKey when 'tRow :> IRow> =
    abstract member TryGetRow: 'tKey -> 'tRow option
    abstract member Items: 'tRow seq
    abstract member Item: 'tKey -> 'tRow with get

type ITable<'tRow, 'tKey when 'tRow :> IRow> =
    inherit IReadonlyTable<'tRow, 'tKey>
    abstract member GenerateKey: unit -> 'tKey
    abstract member Add: 'tRow -> unit
    abstract member Update: 'tRow -> 'tRow -> unit
    abstract member Remove: 'tRow -> unit
    abstract member Item: 'tKey -> 'tRow with get, set
    abstract member RemoveByKey: 'tKey -> unit
