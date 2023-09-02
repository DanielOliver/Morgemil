namespace Morgemil.GameEngine

open Morgemil.Core
open Morgemil.Models
open SadConsole

type ScreenContainer(initialState: ScreenGameState, gameState: IGameBuilder) as this =
    inherit ScreenObject()
    let mutable _state = initialState
    let mutable _selected: IScreenObject = null


    let clearSelected () =
        if _selected <> null then
            _selected.IsVisible <- false
            _selected.IsFocused <- false
            _selected.IsEnabled <- false
            _selected.Children.Clear()
            _selected <- null
            this.Children.Clear()

    override this.Update(timeElapsed: System.TimeSpan) =
        base.Update(timeElapsed)

        match gameState.CurrentState with
        | GameBuilderState.GameBuilt(gameState, initialGameData) ->
            if _state <> ScreenGameState.PlayingGame || _selected = null then
                clearSelected ()
                _selected <- new MapGeneratorConsole(gameState, initialGameData)
                _selected.IsVisible <- true
                _selected.IsFocused <- true
                _selected.IsEnabled <- true
                this.Children.Add(_selected)
                GameHost.Instance.FocusedScreenObjects.Set(_selected)
                _state <- ScreenGameState.PlayingGame
        | GameBuilderState.SelectScenario(scenarios, callback) ->
            if _state <> ScreenGameState.SelectingScenario || _selected = null then
                clearSelected ()

                _selected <-
                    new ScenarioSelectorConsole(
                        scenarios,
                        (fun t ->
                            printfn "Scenario chosen %s" t
                            callback t)
                    )

                _selected.IsVisible <- true
                _selected.IsFocused <- true
                _selected.IsEnabled <- true
                this.Children.Add(_selected)
                GameHost.Instance.FocusedScreenObjects.Set(_selected)
                _state <- ScreenGameState.SelectingScenario

        | GameBuilderState.WaitingForCurrentPlayer addCurrentPlayer -> addCurrentPlayer (AncestryID 1L)
        | _ -> printfn "%A" gameState.CurrentState

    member this.State = _state
