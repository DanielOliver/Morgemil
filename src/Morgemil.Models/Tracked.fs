namespace Morgemil.Models.Tracked

type TrackedEvent<'T> = { OldValue: 'T; NewValue: 'T }

type ITrackedEventHistory<'T> =
    abstract member HistoryCallback: (TrackedEvent<'T> -> unit) with get, set

type ITrackedEntity<'T> =
    abstract member Value: 'T with get, set
    abstract member Get: 'T
    abstract member Set: 'T -> unit

type ITrackedHistory<'T> =
    abstract member HistoryCallback: ('T -> unit) with get, set
