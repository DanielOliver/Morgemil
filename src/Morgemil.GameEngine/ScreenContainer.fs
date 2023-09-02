namespace Morgemil.GameEngine

open Morgemil.Core
open Morgemil.Models
open SadConsole

type ScreenContainer(initialState: ScreenGameState, gameBuilder: IGameBuilder) as this =
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
        | GameBuilderState.GameBuilt(gameState, initialGameData) ->
            if _state <> ScreenGameState.PlayingGame || _selected = null then
                new MapGeneratorConsole(gameState, initialGameData, gameBuilder.Request)
                |> select

                _state <- ScreenGameState.PlayingGame

        | GameBuilderState.SelectScenario(scenarios) ->
            if _state <> ScreenGameState.SelectingScenario || _selected = null then
                new ScenarioSelectorConsole(
                    scenarios,
                    (fun t ->
                        printfn "Scenario chosen %s" t
                        gameBuilder.Request(GameBuilderRequest.SelectScenario t))
                )
                |> select

                _state <- ScreenGameState.SelectingScenario

        | GameBuilderState.WaitingForCurrentPlayer ->
            gameBuilder.Request(GameBuilderRequest.AddCurrentPlayer(AncestryID 1L))

        | _ -> printfn "%A" gameBuilder.CurrentState

        base.Update(timeElapsed)

    member this.State = _state
