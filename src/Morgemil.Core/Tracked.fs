namespace Morgemil.Core

open Morgemil.Models.Tracked

type TrackedEntity<'T when 'T: equality>(initialValue: 'T, history: 'T TrackedEvent -> StepItem) =
    let mutable _recordTrackedEvent = ignore
    let mutable _value = initialValue

    member this.Value = _value

    interface ITrackedHistory with
        member this.HistoryCallback
            with set value = _recordTrackedEvent <- value
            and get () = _recordTrackedEvent

    interface ITrackedEntity<'T> with
        member this.Value
            with get () = _value
            and set x =
                let oldValue = _value
                _value <- x

                if _value <> oldValue then
                    { TrackedEvent.NewValue = _value
                      OldValue = oldValue }
                    |> history
                    |> _recordTrackedEvent

        member this.Get = _value

        member this.Set x =
            let oldValue = _value
            _value <- x

            if _value <> oldValue then
                { TrackedEvent.NewValue = _value
                  OldValue = oldValue }
                |> history
                |> _recordTrackedEvent

module Tracked =
    let Update (entity: 'T :> ITrackedEntity<'U>) (value: 'U) : unit = entity.Set value
    let Replace (entity: 'T :> ITrackedEntity<'U>) (convert: 'U -> 'U) : unit = entity.Get |> convert |> entity.Set
