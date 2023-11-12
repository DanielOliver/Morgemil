namespace Morgemil.GameEngine

open System
open Morgemil.Core
open Morgemil.Models
open Morgemil.Math
open Morgemil.Models.Relational
open SadConsole
open SadConsole.Input

type BasicCrawlConsole
    (
        gameState: IGameStateMachine,
        initialGameData: InitialGameData,
        gameServerRequestCallback: GameServerRequest -> unit
    ) =
    inherit SadConsole.Console(40, 40)

    let timeTable = TimeTable()
    let gameContext = TrackedEntity(initialGameData.GameContext, StepItem.GameContext)
    let character1ID = initialGameData.Characters.[0].ID
    let character1 () = initialGameData.Characters.[0]

    let loopContext: LoopContext =
        { LoopContext.Characters = CharacterTable(timeTable)
          CharacterAttributes = CharacterAttributesTable()
          TimeTable = timeTable
          TileMap = initialGameData.TileMap
          GameContext = gameContext }

    let sidebar = new CrawlSidebar(20, 40, 20, initialGameData, loopContext)
    do base.Children.Add(sidebar)

    let createTileMapFromData (data: TileMapData) =
        let result =
            TileMap(Rectangle(0, 0, data.Size.X, data.Size.Y), data.DefaultTile, Array.zip data.Tiles data.TileFeatures)

        result

    do
        for character in initialGameData.Characters do
            Table.AddRow loopContext.Characters character

        for characterAttributes in initialGameData.CharacterAttributes do
            Table.AddRow loopContext.CharacterAttributes characterAttributes

    member this.Reposition() = sidebar.Reposition()

    override this.ProcessKeyboard(info: Keyboard) : bool =
        if info.IsKeyPressed Keys.Escape then
            gameServerRequestCallback (GameServerRequest.Workflow GameServerWorkflow.ScenarioSelection)
            true
        else
            false


    override this.Update(timeElapsed: TimeSpan) =
        base.Update(timeElapsed)

        let event =
            if SadConsole.GameHost.Instance.Keyboard.IsKeyReleased Keys.Left then
                (character1ID, Point.create (-1, 0)) |> Some
            else if SadConsole.GameHost.Instance.Keyboard.IsKeyReleased Keys.Right then
                (character1ID, Point.create (1, 0)) |> Some
            else if SadConsole.GameHost.Instance.Keyboard.IsKeyReleased Keys.Down then
                (character1ID, Point.create (0, 1)) |> Some
            else if SadConsole.GameHost.Instance.Keyboard.IsKeyReleased Keys.Up then
                (character1ID, Point.create (0, -1)) |> Some
            else
                None
            |> Option.map (fun (characterID, direction) ->
                { ActionRequestMove.CharacterID = characterID
                  ActionRequestMove.Direction = direction }
                |> ActionRequest.Move)
            |> Option.orElseWith (fun () ->
                if SadConsole.GameHost.Instance.Keyboard.IsKeyReleased Keys.LeftShift then
                    character1ID |> ActionRequest.Pause |> Some
                else
                    None)

        let event =
            event
            |> Option.orElseWith (fun () ->
                if SadConsole.GameHost.Instance.Keyboard.IsKeyReleased Keys.Space then
                    character1ID |> ActionRequest.GoToNextLevel |> Some
                else
                    None)

        match gameState.CurrentState with
        | GameState.WaitingForInput(inputCallback) ->
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

                event.Updates
                |> List.iter (fun tableEvent ->
                    match tableEvent with
                    | StepItem.Character character ->
                        match character with
                        | TableEvent.Added(row) -> Table.AddRow loopContext.Characters row
                        | TableEvent.Updated(_, row) -> Table.AddRow loopContext.Characters row
                        | TableEvent.Removed(row) -> Table.RemoveRow loopContext.Characters row
                    | StepItem.CharacterAttributes characterAttributes ->
                        match characterAttributes with
                        | TableEvent.Added(row) -> Table.AddRow loopContext.CharacterAttributes row
                        | TableEvent.Updated(_, row) -> Table.AddRow loopContext.CharacterAttributes row
                        | TableEvent.Removed(row) -> Table.RemoveRow loopContext.CharacterAttributes row
                    | StepItem.GameContext context -> Tracked.Update gameContext context.NewValue
                    | StepItem.CompleteMapChange context -> Tracked.Update loopContext.TileMap context.NewValue
                    | StepItem.TileInstance _ -> failwith "NotImplemented"))

            acknowledgeCallback ()

        sidebar.LoopContext <- loopContext
        base.Clear()

        for tileInstance in loopContext.TileMap.Tiles do
            let (position, tile, tileFeature) =
                (tileInstance.Position, tileInstance.Tile, tileInstance.TileFeature)

            match tileFeature with
            | Some(feature) ->
                let (showFeatureChar, foregroundColor) =
                    if Char.IsWhiteSpace feature.Representation.AnsiCharacter then
                        false,
                        (tile.Representation.ForegroundColor
                         |> Option.defaultValue SadRogue.Primitives.Color.Black)
                    else
                        let foreground =
                            feature.Representation.ForegroundColor
                            |> Option.defaultValue SadRogue.Primitives.Color.TransparentBlack

                        (foreground.A <> (byte 0)), (foreground)

                let backgroundColor =
                    Color.blendColors
                        (feature.Representation.BackGroundColor
                         |> Option.defaultValue SadRogue.Primitives.Color.TransparentBlack)
                        (tile.Representation.BackGroundColor
                         |> Option.defaultValue SadRogue.Primitives.Color.TransparentBlack)

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
                    tile.Representation.BackGroundColor
                    |> Option.defaultValue SadRogue.Primitives.Color.Black

                let foregroundColor =
                    tile.Representation.ForegroundColor
                    |> Option.defaultValue SadRogue.Primitives.Color.White

                base.Cursor.Position <- position

                base.Cursor.Print(
                    ColoredString(tile.Representation.AnsiCharacter.ToString(), foregroundColor, backgroundColor)
                )
                |> ignore

        for (position, character) in loopContext.Characters.ByPositions do
            let color1 = Color.Black

            let representation =
                { TileRepresentation.AnsiCharacter = if character.PlayerID.IsSome then '@' else 'M'
                  BackGroundColor = None
                  ForegroundColor = Some color1 }

            let foregroundColor =
                representation.ForegroundColor
                |> Option.defaultValue SadRogue.Primitives.Color.TransparentBlack

            base.Print(position.X, position.Y, representation.AnsiCharacter.ToString(), foregroundColor)
