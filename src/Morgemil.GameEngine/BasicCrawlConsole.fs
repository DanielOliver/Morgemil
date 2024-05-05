namespace Morgemil.GameEngine

open System
open Morgemil.Core
open Morgemil.Models
open Morgemil.Math
open SadConsole
open SadConsole.Input

type BasicCrawlConsole
    (
        gameState: IGameStateMachine,
        initialGameData: InitialGameData,
        gameServerRequestCallback: GameServerRequest -> unit
    ) =
    inherit Console(40, 40)

    let loopContext = initialGameData.ToLoopContext()

    let sidebar = new CrawlSidebar(20, 40, 20, initialGameData, loopContext)
    do base.Children.Add(sidebar)

    member this.Reposition() = sidebar.Reposition()

    override this.ProcessKeyboard(info: Keyboard) : bool =
        if info.IsKeyPressed Keys.Escape then
            gameServerRequestCallback (GameServerRequest.Workflow GameServerWorkflow.ScenarioSelection)
            true
        else
            false

    override this.Update(timeElapsed: TimeSpan) =
        base.Update(timeElapsed)

        match gameState.CurrentState with
        | GameState.WaitingForInput(waitingOnCharacterID, inputCallback) ->
            let event =
                if GameHost.Instance.Keyboard.IsKeyReleased Keys.Left then
                    (waitingOnCharacterID, Point.create (-1, 0)) |> Some
                else if GameHost.Instance.Keyboard.IsKeyReleased Keys.Right then
                    (waitingOnCharacterID, Point.create (1, 0)) |> Some
                else if GameHost.Instance.Keyboard.IsKeyReleased Keys.Down then
                    (waitingOnCharacterID, Point.create (0, 1)) |> Some
                else if GameHost.Instance.Keyboard.IsKeyReleased Keys.Up then
                    (waitingOnCharacterID, Point.create (0, -1)) |> Some
                else
                    None
                |> Option.map (fun (characterID, direction) ->
                    { ActionRequestMove.CharacterID = characterID
                      ActionRequestMove.Direction = direction }
                    |> ActionRequest.Move)
                |> Option.orElseWith (fun () ->
                    if GameHost.Instance.Keyboard.IsKeyReleased Keys.LeftShift then
                        waitingOnCharacterID |> ActionRequest.Pause |> Some
                    else
                        None)

            let event =
                event
                |> Option.orElseWith (fun () ->
                    if GameHost.Instance.Keyboard.IsKeyReleased Keys.Space then
                        waitingOnCharacterID |> ActionRequest.GoToNextLevel |> Some
                    else
                        None)

            if event.IsSome then
                inputCallback event.Value

        | GameState.Processing -> printfn "processing"
        | GameState.Results(results, acknowledgeCallback) ->
            results
            |> List.iter (fun event ->
                //DEBUG: UNCOMMENT FOR EVENT PRINTING
                // printfn "%A" event

                match event.Event with
                | ActionEvent.MapChange -> printfn "Changed Map"
                | _ -> ()

                event.Updates |> List.iter loopContext.ApplyStepItem)

            acknowledgeCallback ()

    override this.Render(timeElapsed: TimeSpan) =
        base.Render(timeElapsed)
        base.Clear()

        for tileInstance in loopContext.TileMap.Tiles do
            let position, tile, tileFeature =
                (tileInstance.Position, tileInstance.Tile, tileInstance.TileFeature)

            match tileFeature with
            | Some(feature) ->
                let showFeatureChar, foregroundColor =
                    if Char.IsWhiteSpace feature.Representation.AnsiCharacter then
                        false,
                        (tile.Representation.ForegroundColor
                         |> ValueOption.defaultValue SadRogue.Primitives.Color.Black)
                    else
                        let foreground =
                            feature.Representation.ForegroundColor
                            |> ValueOption.defaultValue SadRogue.Primitives.Color.TransparentBlack

                        (foreground.A <> (byte 0)), foreground

                let backgroundColor =
                    Color.blendColors
                        (feature.Representation.BackgroundColor
                         |> ValueOption.defaultValue SadRogue.Primitives.Color.TransparentBlack)
                        (tile.Representation.BackgroundColor
                         |> ValueOption.defaultValue SadRogue.Primitives.Color.TransparentBlack)

                let tileCharacter =
                    if showFeatureChar then
                        feature.Representation.AnsiCharacter.ToString()
                    else
                        tile.Representation.AnsiCharacter.ToString()

                base.Cursor.Position <- position

                base.Cursor.Print(ColoredString(tileCharacter, foregroundColor, backgroundColor))
                |> ignore
            | None ->
                let backgroundColor =
                    tile.Representation.BackgroundColor
                    |> ValueOption.defaultValue SadRogue.Primitives.Color.Black

                let foregroundColor =
                    tile.Representation.ForegroundColor
                    |> ValueOption.defaultValue SadRogue.Primitives.Color.White

                base.Cursor.Position <- position

                base.Cursor.Print(
                    ColoredString(tile.Representation.AnsiCharacter.ToString(), foregroundColor, backgroundColor)
                )
                |> ignore

        for position, character in loopContext.Characters.ByPositions do
            let color1 = Color.Black

            let representation =
                { TileRepresentation.AnsiCharacter = if character.PlayerID.IsSome then '@' else 'M'
                  BackgroundColor = ValueNone
                  ForegroundColor = ValueSome color1 }

            let foregroundColor =
                representation.ForegroundColor
                |> ValueOption.defaultValue SadRogue.Primitives.Color.TransparentBlack

            base.Print(position.X, position.Y, representation.AnsiCharacter.ToString(), foregroundColor)
