namespace Morgemil.Models.Relational

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
    abstract member AddRow: 'tRow -> unit
    abstract member UpdateRow: 'tRow -> 'tRow -> unit
    abstract member Remove: 'tRow -> unit
    abstract member Item: 'tKey -> 'tRow with get, set
    abstract member RemoveByKey: 'tKey -> unit
    abstract member GenerateKey: unit -> 'tKey