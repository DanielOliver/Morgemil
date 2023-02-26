open System
open Morgemil.Core
open Morgemil.Data
open Morgemil.Models
open Morgemil.Math
open Morgemil.Models.Relational
open SadConsole
open SadConsole.Input

let rawGameDataPhase0 =
    Lazy<DTO.RawDtoPhase0>(fun () ->
        JsonReader.ReadGameFiles(
            if System.IO.Directory.Exists "../Morgemil.Data/Game" then
                "../Morgemil.Data/Game"
            else
                "./Game"
        ))

let rawGameDataPhase2 =
    Translation.FromDTO.TranslateFromDtosToPhase2 rawGameDataPhase0.Value

let scenarioData =
    Translation.FromDTO.TranslateFromDtosToScenario rawGameDataPhase0.Value

type MapGeneratorConsole(gameState: IGameStateMachine, initialGameData: InitialGameData) =
    inherit SadConsole.Console(40, 40)

    let timeTable = TimeTable()

    let gameContext = TrackedEntity initialGameData.GameContext

    let character1 = initialGameData.Characters.[0]

    let mutable loopContext: LoopContext =
        { LoopContext.Characters = CharacterTable(timeTable)
          TimeTable = timeTable
          TileMap = initialGameData.TileMap
          GameContext = gameContext }

    let createColor (color: Color) =
        SadRogue.Primitives.Color(color.R, color.G, color.B, color.A)

    let createTileMapFromData (data: TileMapData) =
        let result =
            TileMap(Rectangle.create data.Size, data.DefaultTile, Array.zip data.Tiles data.TileFeatures)

        result

    let blendColors
        (color1: SadRogue.Primitives.Color)
        (color2: SadRogue.Primitives.Color)
        : SadRogue.Primitives.Color =
        if color1.A = Byte.MaxValue || color2.A = Byte.MinValue then
            color1
        elif color1.A = Byte.MinValue then
            color2
        else
            let ratio = ((float32) color2.A) / ((float32) color1.A + (float32) color2.A)

            let returnColor = SadRogue.Primitives.Color.Lerp(color1, color2, ratio)

            SadRogue.Primitives.Color(returnColor, 255)


    do
        for character in initialGameData.Characters do
            Table.AddRow loopContext.Characters character

    override this.Update(timeElapsed: TimeSpan) =
        let event =
            if SadConsole.GameHost.Instance.Keyboard.IsKeyReleased Keys.Left then
                (character1.ID, Vector2i.create (-1, 0)) |> Some
            else if SadConsole.GameHost.Instance.Keyboard.IsKeyReleased Keys.Right then
                (character1.ID, Vector2i.create (1, 0)) |> Some
            else if SadConsole.GameHost.Instance.Keyboard.IsKeyReleased Keys.Down then
                (character1.ID, Vector2i.create (0, 1)) |> Some
            else if SadConsole.GameHost.Instance.Keyboard.IsKeyReleased Keys.Up then
                (character1.ID, Vector2i.create (0, -1)) |> Some
            else
                None
            |> Option.map (fun (characterID, direction) ->
                { ActionRequestMove.CharacterID = characterID
                  ActionRequestMove.Direction = direction }
                |> ActionRequest.Move)

        let event =
            event
            |> Option.orElseWith (fun () ->
                if SadConsole.GameHost.Instance.Keyboard.IsKeyReleased Keys.Space then
                    character1.ID |> ActionRequest.GoToNextLevel |> Some
                else
                    None)

        match gameState.CurrentState with
        | GameState.WaitingForInput (inputCallback) ->
            if event.IsSome then
                inputCallback event.Value
        | GameState.Processing -> printfn "processing"
        | GameState.Results (results, acknowledgeCallback) ->
            results
            |> List.iter (fun event ->
                printfn "%A" event

                match event.Event with
                | ActionEvent.MapChange mapChange ->
                    let timeTable = TimeTable()

                    loopContext <-
                        { loopContext with
                            TileMap = createTileMapFromData mapChange.TileMapData
                            Characters = CharacterTable(timeTable)
                            TimeTable = timeTable }

                    mapChange.Characters |> Array.iter (Table.AddRow loopContext.Characters)
                | _ ->
                    event.Updates
                    |> List.iter (fun tableEvent ->
                        match tableEvent with
                        | StepItem.Character character ->
                            match character with
                            | TableEvent.Added (row) -> Table.AddRow loopContext.Characters row
                            | TableEvent.Updated (_, row) -> Table.AddRow loopContext.Characters row
                            | TableEvent.Removed (row) -> Table.RemoveRow loopContext.Characters row
                        | StepItem.GameContext context -> Tracked.Update gameContext context.NewValue))

            acknowledgeCallback ()

        base.Clear()

        for tileInstance in loopContext.TileMap.Tiles do
            let (position, tile, tileFeature) =
                (tileInstance.Position, tileInstance.Tile, tileInstance.TileFeature)

            match tileFeature with
            | Some (feature) ->
                let (showFeatureChar, foregroundColor) =
                    if Char.IsWhiteSpace feature.Representation.AnsiCharacter then
                        false,
                        (tile.Representation.ForegroundColor
                         |> Option.map createColor
                         |> Option.defaultValue SadRogue.Primitives.Color.Black)
                    else
                        let foreground =
                            feature.Representation.ForegroundColor
                            |> Option.map createColor
                            |> Option.defaultValue SadRogue.Primitives.Color.TransparentBlack

                        (foreground.A <> (byte 0)), (foreground)

                let backgroundColor =
                    blendColors
                        (feature.Representation.BackGroundColor
                         |> Option.map createColor
                         |> Option.defaultValue SadRogue.Primitives.Color.TransparentBlack)
                        (tile.Representation.BackGroundColor
                         |> Option.map createColor
                         |> Option.defaultValue SadRogue.Primitives.Color.TransparentBlack)

                let tileCharacter =
                    if showFeatureChar then
                        feature.Representation.AnsiCharacter.ToString()
                    else
                        tile.Representation.AnsiCharacter.ToString()

                base.Cursor.Position <- SadRogue.Primitives.Point(position.X, position.Y)

                base.Cursor.Print(ColoredString(tileCharacter, foregroundColor, backgroundColor))
                |> ignore
            | None ->
                let backgroundColor =
                    tile.Representation.BackGroundColor
                    |> Option.map createColor
                    |> Option.defaultValue SadRogue.Primitives.Color.Black

                let foregroundColor =
                    tile.Representation.ForegroundColor
                    |> Option.map createColor
                    |> Option.defaultValue SadRogue.Primitives.Color.White

                base.Cursor.Position <- SadRogue.Primitives.Point(position.X, position.Y)

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
                |> Option.map createColor
                |> Option.defaultValue SadRogue.Primitives.Color.TransparentBlack

            base.Print(position.X, position.Y, representation.AnsiCharacter.ToString(), foregroundColor)

let StartMainStateMachine () =
    let mainGameState =
        new SimpleGameBuilderMachine(DataLoader.LoadScenarioData) :> IGameBuilder

    let mutable gameHasRun = true

    while gameHasRun do
        match mainGameState.CurrentState with
        | GameBuilderState.GameBuilt (gameState, initialGameData) ->
            gameHasRun <- false

            let Init () =
                let gameConsole = new MapGeneratorConsole(gameState, initialGameData)

                gameConsole.UseKeyboard <- true
                SadConsole.Settings.WindowTitle <- "Morgemil"
                SadConsole.GameHost.Instance.Screen <- gameConsole

            SadConsole.Game.Create(80, 40, "Cheepicus12.font")
            SadConsole.Game.Instance.OnStart <- new Action(Init)
            SadConsole.Game.Instance.Run()
            SadConsole.Game.Instance.Dispose()
        | GameBuilderState.SelectScenario (scenarios, callback) ->
            printfn "Scenarios: "

            scenarios
            |> Seq.iteri (fun index scenarioName -> printfn "%-5i | %s" index scenarioName)

            printfn "Choose Scenario: "
            Console.ReadLine() |> callback
        | GameBuilderState.LoadedScenarioData _ -> failwith "one"
        | GameBuilderState.WaitingForCurrentPlayer addCurrentPlayer -> addCurrentPlayer (AncestryID 1L)
        | GameBuilderState.LoadingGameProgress status -> printfn "Status %s" status

    ()

[<EntryPoint>]
let main argv =
    printfn "Starting Morgemil"
    StartMainStateMachine()
    0 // return an integer exit code
