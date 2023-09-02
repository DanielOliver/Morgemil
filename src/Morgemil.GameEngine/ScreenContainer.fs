namespace Morgemil.GameEngine

open Morgemil.Core
open Morgemil.Models
open SadConsole

type ScreenContainer(initialState: ScreenGameState, gameBuilder: IGameServer) as this =
    inherit ScreenObject()
    let mutable _state = initialState
    let mutable _selected: IScreenObject = null

    let clearSelected () =
        if _selected <> null then
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
        match gameBuilder.CurrentState with
        | GameServerState.GameBuilt(gameState, initialGameData) ->
            if _state <> ScreenGameState.PlayingGame || _selected = null then

                new BasicCrawlConsole(gameState, initialGameData, gameBuilder.Request) |> select

                _state <- ScreenGameState.PlayingGame

        | GameServerState.SelectScenario(scenarios) ->
            if _state <> ScreenGameState.SelectingScenario || _selected = null then
                new ScenarioSelectorConsole(
                    scenarios,
                    (fun t ->
                        printfn "Scenario chosen %s" t
                        gameBuilder.Request(GameServerRequest.SelectScenario t))
                )
                |> select

                _state <- ScreenGameState.SelectingScenario

        | GameServerState.WaitingForCurrentPlayer ->
            gameBuilder.Request(GameServerRequest.AddCurrentPlayer(AncestryID 1L))

        | _ -> printfn "%A" gameBuilder.CurrentState

        base.Update(timeElapsed)

    member this.State = _state
