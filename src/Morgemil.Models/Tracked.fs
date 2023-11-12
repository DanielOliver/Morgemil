namespace Morgemil.Models.Tracked

type TrackedEvent<'T> = { OldValue: 'T; NewValue: 'T }

type ITrackedEntity<'T> =
    abstract member Value: 'T with get, set
    abstract member Get: 'T
    abstract member Set: 'T -> unit
