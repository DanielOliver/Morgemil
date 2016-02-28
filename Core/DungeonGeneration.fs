namespace Morgemil.Core

open Morgemil.Core
open Morgemil.Math
open System

///Creates a Dungeon
module DungeonGeneration = 
  /// The existence of a mutable structure is necessary as a dungeon requires multiple passes to generate
  type private DungeonMap(roomSize : Rectangle, depth : int) = 
    let internal_map = Array.create roomSize.Area (Tiles.DungeonWall)
    member this.SetValue tile pos = internal_map.[roomSize.FlatCoord pos] <- tile
    ///Return a single giant level to feed into the Visualizer
    member this.CreateLevel() = 
      { Area = roomSize
        Tiles = internal_map
        TileModifiers = List.empty
        Depth = depth }
  
  ///The absolute minimum room area tolerated
  let private minimumRoomArea = Vector2i.From(13)
  
  ///Randomizes a room's position and size within the defined area
  let private randomizeRoom rng (maxArea : Rectangle) = 
    //Randomize size before placing the room randomly
    let room_size = minimumRoomArea + RNG.RandomVector rng (maxArea.Size - minimumRoomArea) - 2
    let room_position = maxArea.Position + 1 + RNG.RandomVector rng (maxArea.Size - room_size - 2)
    Rectangle.From(room_position, room_size)
  
  ///Writes directly on the map (side-effects). returns nothing
  let private generateRoom (dungeonMap : DungeonMap) (area : Rectangle) = 
    area.Coordinates |> Seq.iter (dungeonMap.SetValue Tiles.DungeonFloor)
  
  ///Writes directly on the map (side-effects). returns nothing
  let private generateCorridor (dungeonMap : DungeonMap) (area : Rectangle) = 
    area.Coordinates |> Seq.iter (dungeonMap.SetValue Tiles.DungeonCorridor)
  
  let private shareHorizontal (rect1 : Rectangle) (rect2 : Rectangle) = 
    not (rect1.Left > rect2.Right || rect1.Right < rect2.Left)
  let private shareVertical (rect1 : Rectangle) (rect2 : Rectangle) = 
    not (rect1.Top > rect2.Bottom || rect1.Bottom < rect2.Top)
  
  ///Assuming non-intersecting rectangles
  let private shareAxis (rect1 : Rectangle) (rect2 : Rectangle) = 
    match rect1 with
    | _ when shareHorizontal rect1 rect2 -> Some(Axis.Horizontal)
    | _ when shareVertical rect1 rect2 -> Some(Axis.Vertical)
    | _ -> None
  
  let private sortVertical (rect1 : Rectangle) (rect2 : Rectangle) = 
    match rect1.Bottom < rect2.Top with
    | true -> rect1, rect2
    | false -> rect2, rect1
  
  let private sortHorizontal (rect1 : Rectangle) (rect2 : Rectangle) = 
    match rect1.Right < rect2.Left with
    | true -> rect1, rect2
    | false -> rect2, rect1
  
  ///Returns success and a axis-aligned corridor between the two rectangles if it exists.
  let private corridor (rect1 : Rectangle) (rect2 : Rectangle) = 
    match shareAxis rect1 rect2 with
    | None -> None
    | Some(ax) -> 
      match ax with
      | Axis.Horizontal -> 
        let first, second = sortVertical rect1 rect2
        let pos1 = Vector2i.From(Math.Max(first.Left, second.Left), first.Bottom + 1)
        
        ///Make corridor of width 2 if possible. But also handles a corridor of one width
        let minX = Math.Min(pos1.X + 1, Math.Min(first.Right, second.Right))
        
        let pos2 = Vector2i.From(minX, second.Top - 1)
        Some(Rectangle.Enclose pos1 pos2)
      | Axis.Vertical -> 
        let first, second = sortHorizontal rect1 rect2
        let pos1 = Vector2i.From(first.Right + 1, Math.Max(first.Top, second.Top))
        
        ///Make corridor of heighth 2 if possible. But also handles a corridor of one height
        let minY = Math.Min(pos1.Y + 1, Math.Min(first.Bottom, second.Bottom))
        
        let pos2 = Vector2i.From(second.Left - 1, minY)
        Some(Rectangle.Enclose pos1 pos2)
  
  ///For each room, tests it against every room after it
  let rec private createRoomCorridors (L : list<Rectangle>) = 
    match L with
    | head :: tail -> List.choose (corridor head) tail |> List.append (createRoomCorridors tail)
    | [] -> []
  
  let private generateBSP (param : DungeonParameter) = 
    let rng = RNG.SeedRNG param.RngSeed
    //Hardcoded dungeon size
    let dungeon_size = Rectangle.From(Vector2i.From(124, 90))
    //Empty map
    let dungeon_map = DungeonMap(dungeon_size, param.Depth)
    //Room params
    let min_room_size = minimumRoomArea + 5
    let max_room_size = Vector2i.From(25)
    //BSP rooms with randomized size (Rectangle List).
    let dungeonRooms = 
      BspGenerator(min_room_size, max_room_size, dungeon_size.Size).GenerateRoomDivides rng 
      |> List.map (randomizeRoom rng)
    //Feed the randomized rooms to the map
    dungeonRooms |> List.iter (generateRoom dungeon_map)
    ///Tests for collisions with rooms
    let collides (corr : Rectangle) = dungeonRooms |> List.exists (corr.Intersects)
    //Draw the corridors on the map
    dungeonRooms
    |> createRoomCorridors
    |> List.filter (collides >> not)
    |> List.iter (generateCorridor dungeon_map)
    //Return a level to feed to visualizer
    dungeon_map.CreateLevel()
  
  let private generateSquare (param : DungeonParameter) = 
    let rng = RNG.SeedRNG param.RngSeed
    //Slightly randomized dungeon size
    let dungeon_size = Rectangle.From(Vector2i.From(60) + RNG.RandomVector rng (Vector2i.From(60)))
    //Empty map
    let dungeon_map = DungeonMap(dungeon_size, param.Depth)
    generateRoom dungeon_map (dungeon_size.Expand -1)
    //Return a level to feed to visualizer
    let center = dungeon_size.MinCoord + (dungeon_size.Size / 2)
    //Creates an empty dungeon level
    let level = dungeon_map.CreateLevel()
    //Take the first passable tile as the level entrance
    let (entrance_coord, _) = level.TileCoordinates |> Seq.find (fun (vec2, tile) -> not (tile.BlocksMovement))
    { level with TileModifiers = 
                   [ TileModifier.Stairs { DungeonParameter = 
                                             { Type = DungeonGenerationType.Square
                                               Depth = param.Depth + 1
                                               RngSeed = rng.Next() }
                                           Area = Rectangle.From(center, Vector2i.From(1)) }
                     TileModifier.Entrance(Rectangle.From(entrance_coord, Vector2i.From(1))) ] }
  
  let Generate(param : DungeonParameter) = 
    match param.Type with
    | DungeonGenerationType.BSP -> generateBSP param
    | DungeonGenerationType.Square -> generateSquare param
