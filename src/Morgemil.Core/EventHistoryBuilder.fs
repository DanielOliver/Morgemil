namespace Morgemil.Core

open System
open Morgemil.Core
open Morgemil.Models

type EventHistoryBuilder(characterTable: CharacterTable, gameContext: TrackedEntity<GameContext>) =
    let mutable _events: StepItem list = []
    let mutable isDisposed = false

    let characterTableCallback = Table.GetHistoryCallback characterTable

    let gameContextCallback =
        Tracked.GetHistoryCallback gameContext

    do
        Table.SetHistoryCallback characterTable (fun t -> _events <- (t |> StepItem.Character) :: _events)
        Tracked.SetHistoryCallback gameContext (fun t -> _events <- (t |> StepItem.GameContext) :: _events)

    member this.Bind(m, f) = f m

    member this.Return(x: ActionEvent) : Step list =
        let step = { Step.Event = x; Updates = _events }
        _events <- []
        [ step ]

    member this.Return(x: Step list) : Step list = x
    member this.Zero() : Step list = []

    member this.Yield(x: ActionEvent) : Step list =
        let step = { Step.Event = x; Updates = _events }
        _events <- []
        [ step ]

    member this.Yield(x: Step list) : Step list = x
    member this.Combine(a: Step list, b: Step list) : Step list = List.concat [ b; a ]
    member this.Delay(f) = f ()

    interface IDisposable with
        member this.Dispose() =
            if not isDisposed then
                Table.SetHistoryCallback characterTable characterTableCallback
                Tracked.SetHistoryCallback gameContext gameContextCallback
                isDisposed <- true
