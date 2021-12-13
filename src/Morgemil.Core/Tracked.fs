namespace Morgemil.Core

open Morgemil.Core
open Morgemil.Models.Tracked

type TrackedEntity<'T>(initialValue: 'T) =
    let mutable _recordEvent = ignore
    let mutable _value = initialValue

    interface ITrackedEventHistory<'T> with
        member this.HistoryCallback
            with get () = _recordEvent
            and set x = _recordEvent <- x

    interface ITrackedEntity<'T> with
        member this.Value
            with get () = _value
            and set x =
                let oldValue = _value
                _value <- x

                _recordEvent
                    { TrackedEvent.NewValue = _value
                      OldValue = oldValue }

        member this.Get = _value

        member this.Set x =
            let oldValue = _value
            _value <- x

            _recordEvent
                { TrackedEvent.NewValue = _value
                  OldValue = oldValue }

module TrackedEntity =
    let GetHistoryCallback (entity: 'T :> ITrackedEventHistory<'U>) : (TrackedEvent<'U> -> unit) =
        entity.HistoryCallback

    let SetHistoryCallback (entity: 'T :> ITrackedEventHistory<'U>) (callback: (TrackedEvent<'U> -> unit)) : unit =
        entity.HistoryCallback <- callback

    let Update (entity: 'T :> ITrackedEntity<'U>) (value: 'U) : unit = entity.Set value
    let Replace (entity: 'T :> ITrackedEntity<'U>) (convert: 'U -> 'U) : unit = entity.Get |> convert |> entity.Set
