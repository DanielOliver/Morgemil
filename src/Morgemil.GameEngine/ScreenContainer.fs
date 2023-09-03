namespace Morgemil.GameEngine

open Morgemil.Core
open Morgemil.Models
open SadConsole

type ScreenContainer(initialState: ScreenGameState, gameServer: IGameServer) as this =
    inherit ScreenObject()
    let mutable _state = initialState
    let mutable _selected: IScreenObject = null
    let mutable _reposition: (unit -> unit) = (fun () -> ())

    let clearSelected () =
        if _selected <> null then
            _reposition <- (fun () -> ())
            _selected.IsVisible <- false
            // _selected.IsFocused <- false
            _selected.IsEnabled <- false
            _selected.Children.Clear()
            _selected <- null
            this.Children.Clear()

    let select (selected: IScreenObject) =
        clearSelected ()
        _selected <- selected
        _selected.IsVisible <- true
        _selected.IsFocused <- true
        _selected.IsEnabled <- true
        this.Children.Add(_selected)
        GameHost.Instance.FocusedScreenObjects.Set(_selected)

    override this.Update(timeElapsed: System.TimeSpan) =
        match gameServer.CurrentState with
        | GameServerState.GameBuilt(gameState, initialGameData) ->
            if _state <> ScreenGameState.PlayingGame || _selected = null then

                let crawl = new BasicCrawlConsole(gameState, initialGameData, gameServer.Request)
                crawl |> select
                _reposition <- crawl.Reposition

                _state <- ScreenGameState.PlayingGame

        | GameServerState.SelectScenario(scenarios) ->
            if _state <> ScreenGameState.SelectingScenario || _selected = null then
                new ScenarioSelectorConsole(
                    scenarios,
                    (fun t ->
                        printfn "Scenario chosen %s" t
                        gameServer.Request(GameServerRequest.SelectScenario t))
                )
                |> select

                _state <- ScreenGameState.SelectingScenario

        | GameServerState.WaitingForCurrentPlayer ->
            gameServer.Request(GameServerRequest.AddCurrentPlayer(AncestryID 1L))

        | _ -> printfn "%A" gameServer.CurrentState

        base.Update(timeElapsed)

    member this.State = _state

    member this.Reposition() = _reposition ()
