namespace Morgemil.Core

open System
open Morgemil.Core
open Morgemil.Models

type EventHistoryBuilder(tracked: ITrackedHistory list) =
    let mutable _events: StepItem list = []
    let mutable isDisposed = false
    let historyCallbacks = tracked |> List.map (fun t -> (t, t.HistoryCallback))

    do
        historyCallbacks
        |> List.iter (fun (tracked, _) -> tracked.HistoryCallback <- (fun t -> _events <- t :: _events))

    member this.Bind(m, f) = f m

    member this.Return(x: ActionEvent) : Step list =
        let step =
            { Step.Event = x
              Updates = _events |> List.rev }

        _events <- []
        [ step ]

    member this.Return(x: Step list) : Step list = x
    member this.Zero() : Step list = []

    member this.Yield(x: ActionEvent) : Step list =
        match x with
        | ActionEvent.Empty _ when _events.IsEmpty -> []
        | _ ->
            let step =
                { Step.Event = x
                  Updates = _events |> List.rev }

            _events <- []
            [ step ]

    member this.Yield(x: Step list) : Step list = x
    member this.Combine(a: Step list, b: Step list) : Step list = List.concat [ b; a ]
    member this.Delay(f) = f ()

    interface IDisposable with
        member this.Dispose() =
            if not isDisposed then
                historyCallbacks
                |> List.iter (fun (tracked, callback) -> tracked.HistoryCallback <- callback)

                isDisposed <- true
