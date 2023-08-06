namespace Morgemil.GameEngine

open SadConsole

type ScreenContainer(initialState: ScreenGameState) as this =
    inherit ScreenObject()
    let mutable _state = initialState
    let mutable _selected: IScreenObject = null

    do this.TransitionState(initialState)

    member this.TransitionState(state: ScreenGameState) =
        if state = _state && _selected <> null then
            //No transition to a new state required.
            ()
        else
            if _selected <> null then
                _selected.IsVisible <- false
                _selected.IsFocused <- false
                _selected.IsEnabled <- false
                _selected.Children.Clear()
                _selected <- null
                this.Children.Clear()

            match state with
            | MapGeneratorConsole(gameStateMachine, initialGameData) ->
                _selected <- new MapGeneratorConsole(gameStateMachine, initialGameData)
                _selected.IsVisible <- true
                _selected.IsFocused <- true
                _selected.IsEnabled <- true
                this.Children.Add(_selected)
                GameHost.Instance.FocusedScreenObjects.Set(_selected)
            | _ -> ()

            _state <- state

    member this.State = _state
