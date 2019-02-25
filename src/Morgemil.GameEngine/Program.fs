// Learn more about F# at http://fsharp.org

open System
open Morgemil.Core
open Morgemil.Models
open Morgemil.Math

let defaultTile: Tile = {
        ID = TileID 3L
        Name = "Dungeon Wall"
        TileType = TileType.Solid
        Description = "Dungeon Walls are rock, solid, and carved from time."
        BlocksMovement = true
        BlocksSight = true
        Representation = {
            AnsiCharacter = char(177)
            ForegroundColor = Some <| Color.From(100, 100, 100)
            BackGroundColor = Some <| Color.From(30, 30, 30)
        }
    }

let floorTile: Tile = {
        ID = TileID 4L
        Name = "Dungeon Floor"
        TileType = TileType.Ground
        Description = "Dungeon floors are rock, paved cobblestone, and very slipper when bloody."
        BlocksMovement = false
        BlocksSight = false
        Representation = {
            AnsiCharacter = ' '
            ForegroundColor = Some <| Color.From(50, 50, 50)
            BackGroundColor = Some <| Color.From(255, 255, 255)
        }
    }

let stairTileFeature: TileFeature = {
        ID = TileFeatureID 2L
        Name = "Stairs down"
        Description = "Stairs down"
        BlocksMovement = false
        BlocksSight = false
        Representation = {
            AnsiCharacter = char(242)
            ForegroundColor = Some <| Color.From(30, 30, 255)
            BackGroundColor = Some <| Color.From(0, 240, 0, 50)
        }
        PossibleTiles = [
            defaultTile
            floorTile
        ]
    }

let startingPointFeature: TileFeature = {
        ID = TileFeatureID 1L
        Name = "Starting point"
        Description = "Starting point"
        BlocksMovement = false
        BlocksSight = false
        Representation = {
            AnsiCharacter = '@'
            ForegroundColor = Some <| Color.From(0)
            BackGroundColor = None
        }
        PossibleTiles = [
            defaultTile
            floorTile
        ]
    }

let floorParameters: FloorGenerationParameter = {
    Strategy = FloorGenerationStrategy.OpenFloor
    Tiles = [|
        defaultTile
        floorTile
    |]
    SizeRange = Rectangle.create(10, 10, 15, 15)
    DefaultTile = defaultTile
    ID = FloorGenerationParameterID 5L
}
    


type MapGeneratorConsole() =
    inherit SadConsole.Console(40, 40)

    let rng = RNG.SeedRNG(50)
    let (tileMap, results) = FloorGenerator.Create floorParameters rng
    let createColor (color: Morgemil.Math.Color) = Microsoft.Xna.Framework.Color(color.R, color.G, color.B, color.A)

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
        tileMap.TileFeature(Vector2i.create 10) <- (Some stairTileFeature)
        tileMap.TileFeature(Vector2i.create 3) <- (Some startingPointFeature)

        for (position, tile, tileFeature) in tileMap.Tiles do
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

    member this.Zero = ()


let Init() = 
    //SadConsole.Game.Instance.Components.Add(new SadConsole.Game.FPSCounterComponent(SadConsole.Game.Instance))
    SadConsole.Game.Instance.Window.Title <- "Morgemil";
    SadConsole.Global.CurrentScreen <- (new MapGeneratorConsole())
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


