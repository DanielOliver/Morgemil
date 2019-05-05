open System
open Morgemil.Core
open Morgemil.Data
open Morgemil.Models
open Morgemil.Math
open Morgemil.Models.Relational
open Microsoft.Xna.Framework.Input

let rawGameDataPhase0 = Lazy<DTO.RawDtoPhase0>(fun () -> JsonReader.ReadGameFiles ( if System.IO.Directory.Exists "../Morgemil.Data/Game" then "../Morgemil.Data/Game" else "./Game"))
let rawGameDataPhase2 = Translation.FromDTO.TranslateFromDtosToPhase2 rawGameDataPhase0.Value
let scenarioData = Translation.FromDTO.TranslateFromDtosToScenario rawGameDataPhase0.Value

type MapGeneratorConsole() =
    inherit SadConsole.Console(40, 40)

    let rng = RNG.SeedRNG(50)
    let tileFeatureTable =
        rawGameDataPhase2.TileFeatures
        |> Morgemil.Core.TileFeatureTable
    let readonlyTileTable = scenarioData.Tiles
    let (tileMap, results) = FloorGenerator.Create rawGameDataPhase2.FloorGenerationParameters.[0] tileFeatureTable rng
    let createColor (color: Morgemil.Math.Color) = Microsoft.Xna.Framework.Color(color.R, color.G, color.B, color.A)

    let createTileMapFromData(data: TileMapData) =
        let result =
            TileMap(
                Rectangle.create data.Size,
                data.DefaultTile,
                Array.zip data.Tiles data.TileFeatures)

        result

    let mutable viewOnlyTileMap = createTileMapFromData tileMap.TileMapData
    let characterTable = CharacterTable()
    let mutable viewCharacterTable = CharacterTable()
    let character1 = {
        Character.ID = Table.GenerateKey characterTable
        Race = rawGameDataPhase2.Races.[0]
        RaceModifier = None
        Position = Vector2i.create(5)
        PlayerID = None
    }

    let gameLoop = Loop(characterTable, tileMap, scenarioData)
    let gameState = SimpleGameStateMachine(gameLoop.ProcessRequest, scenarioData) :> IGameStateMachine

    let blendColors (color1: Microsoft.Xna.Framework.Color) (color2: Microsoft.Xna.Framework.Color) =
        if color1.A = System.Byte.MaxValue || color2.A = System.Byte.MinValue then
            color1
        elif color1.A = System.Byte.MinValue then
            color2
        else
            let ratio = ((float32)color2.A) / ((float32)color1.A + (float32)color2.A)
            let returnColor = Microsoft.Xna.Framework.Color.Lerp(color1, color2, ratio)
            Microsoft.Xna.Framework.Color(returnColor, 255)


    do
        Table.AddRow characterTable character1
        Table.AddRow viewCharacterTable character1

    override this.Update(timeElapsed: TimeSpan) =
        let event =
            if SadConsole.Global.KeyboardState.IsKeyReleased Keys.Left then
                (character1.ID, Vector2i.create(-1, 0)) |> Some
            else if SadConsole.Global.KeyboardState.IsKeyReleased Keys.Right then
                (character1.ID, Vector2i.create(1, 0)) |> Some
            else if SadConsole.Global.KeyboardState.IsKeyReleased Keys.Down then
                (character1.ID, Vector2i.create(0, 1)) |> Some
            else if SadConsole.Global.KeyboardState.IsKeyReleased Keys.Up then
                (character1.ID, Vector2i.create(0, -1)) |> Some
            else
                None
            |> Option.map (fun (characterID, direction) ->
                {
                    ActionRequestMove.CharacterID = characterID
                    ActionRequestMove.Direction = direction
                }
                |> ActionRequest.Move
                )

        let event =
            event
            |> Option.orElseWith(fun () ->
                if SadConsole.Global.KeyboardState.IsKeyReleased Keys.Space then
                    character1.ID
                    |> ActionRequest.GoToNextLevel
                    |> Some
                else None
            )

        match gameState.CurrentState with
        | GameState.WaitingForInput (inputCallback) ->
            if event.IsSome then
                inputCallback event.Value
        | GameState.Processing ->
            printfn "processing"
        | GameState.Results (results, acknowledgeCallback) ->
            results
            |> List.iter (fun event ->
                printfn "%A" event
                match event.Event with
                | ActionEvent.MapChange mapChange ->
                    viewOnlyTileMap <- createTileMapFromData mapChange.TileMapData
                    viewCharacterTable <- CharacterTable()
                    mapChange.Characters
                    |> Array.iter (Table.AddRow viewCharacterTable)
                | _ ->
                    event.Updates
                    |> List.iter(fun tableEvent ->
                        match tableEvent with
                        | StepItem.Character character ->
                            match character with
                            | TableEvent.Added(row) -> Table.AddRow viewCharacterTable row
                            | TableEvent.Updated(_, row) -> Table.AddRow viewCharacterTable row
                            | TableEvent.Removed(row) -> Table.RemoveRow viewCharacterTable row
                    )
                )
            acknowledgeCallback()


        for (position, tile, tileFeature) in viewOnlyTileMap.Tiles do
            match tileFeature with
            | Some(feature) ->
                let (showFeatureChar, foregroundColor) =
                    if System.Char.IsWhiteSpace feature.Representation.AnsiCharacter then
                        false, (tile.Representation.ForegroundColor |> Option.map createColor |> Option.defaultValue Microsoft.Xna.Framework.Color.Black)
                    else
                        let foreground = feature.Representation.ForegroundColor |> Option.map createColor |> Option.defaultValue Microsoft.Xna.Framework.Color.TransparentBlack
                        (foreground.A <> (byte 0)), (foreground)

                let backgroundColor =
                    blendColors
                        (feature.Representation.BackGroundColor |> Option.map createColor |> Option.defaultValue Microsoft.Xna.Framework.Color.TransparentBlack)
                        (tile.Representation.BackGroundColor |> Option.map createColor |> Option.defaultValue Microsoft.Xna.Framework.Color.TransparentBlack)

                let tileCharacter = if showFeatureChar then feature.Representation.AnsiCharacter.ToString() else tile.Representation.AnsiCharacter.ToString()
                base.Print(position.X, position.Y, tileCharacter, foregroundColor, backgroundColor)
            | None ->
                let backgroundColor = tile.Representation.BackGroundColor |> Option.map createColor |> Option.defaultValue Microsoft.Xna.Framework.Color.Black
                let foregroundColor = tile.Representation.ForegroundColor |> Option.map createColor |> Option.defaultValue Microsoft.Xna.Framework.Color.White
                base.Print(position.X, position.Y, tile.Representation.AnsiCharacter.ToString(), foregroundColor, backgroundColor)

        for (position, character) in viewCharacterTable.ByPositions do
            let color1 = Color.Black
            let representation = {
                TileRepresentation.AnsiCharacter = '@'
                BackGroundColor = None
                ForegroundColor = Some color1
            }
            let foregroundColor = representation.ForegroundColor |> Option.map createColor |> Option.defaultValue Microsoft.Xna.Framework.Color.TransparentBlack
            base.Print(position.X, position.Y, representation.AnsiCharacter.ToString(), foregroundColor)

let Init() =
    let gameConsole = new MapGeneratorConsole()
    gameConsole.UseKeyboard <- true
    gameConsole.KeyboardHandler <- null
    //SadConsole.Game.Instance.Components.Add(new SadConsole.Game.FPSCounterComponent(SadConsole.Game.Instance))
    SadConsole.Game.Instance.Window.Title <- "Morgemil";
    SadConsole.Global.CurrentScreen <- gameConsole
    //SadConsole.Global.CurrentScreen.Position <- Microsoft.Xna.Framework.Point(1,1)
    ()

[<EntryPoint>]
let main argv =
    printfn "Starting Morgemil"
    SadConsole.Game.Create("Cheepicus12.font", 80, 40);
    SadConsole.Game.OnInitialize <- new Action(Init)
    SadConsole.Game.Instance.Run();
    SadConsole.Game.Instance.Dispose();
    0 // return an integer exit code


