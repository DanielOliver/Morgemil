namespace Morgemil.Map

open Morgemil.Math
open System

///Creates a Dungeon
module DungeonGeneration =
  /// The existence of a mutable structure is necessary as a dungeon requires multiple passes to generate
  type private DungeonMap(roomSize : Rectangle) =
    let internal_map = Array.create roomSize.Area (Tiles.DungeonWall)
    member this.SetValue pos tile = internal_map.[roomSize.FlatCoord pos] <- tile
    ///Return a single giant chunk to feed into the Visualizer
    member this.CreateChunk() = Chunk(roomSize, internal_map)

  ///The absolute minimum room area tolerated
  let private MinimumRoomArea = Vector2i(21, 21)

  ///Randomizes a room's position and size within the defined area
  let private RandomizeRoom rng (maxArea : Rectangle) =
    //Randomize size before placing the room randomly
    let room_size = MinimumRoomArea + RNG.RandomVector rng (maxArea.Size - MinimumRoomArea)
    let room_position = maxArea.Position + RNG.RandomVector rng (maxArea.Size - room_size)
    Rectangle(room_position, room_size)

  ///Writes directly on the map (side-effects). returns nothing
  let private GenerateRoom (dungeonMap : DungeonMap) (area : Rectangle) =
    area.Coordinates |> Seq.iter (fun coord -> dungeonMap.SetValue coord Tiles.DungeonFloor)

  ///Writes directly on the map (side-effects). returns nothing
  let private GenerateCorridor (dungeonMap : DungeonMap) (area : Rectangle) =
    area.Coordinates |> Seq.iter (fun coord -> dungeonMap.SetValue coord Tiles.DungeonCorridor)

  let ShareHorizontal (rect1 : Rectangle) (rect2 : Rectangle) =
    not (rect1.Left > rect2.Right || rect1.Right < rect2.Left)
  let ShareVertical (rect1 : Rectangle) (rect2 : Rectangle) =
    not (rect1.Top > rect2.Bottom || rect1.Bottom < rect2.Top)

  ///Assuming non-intersecting rectangles
  let ShareAxis (rect1 : Rectangle) (rect2 : Rectangle) =
    match rect1 with
    | _ when ShareHorizontal rect1 rect2 -> Some(Axis.Horizontal)
    | _ when ShareVertical rect1 rect2 -> Some(Axis.Vertical)
    | _ -> None

  let SortVertical (rect1 : Rectangle) (rect2 : Rectangle) =
    match rect1.Bottom < rect2.Top with
    | true -> rect1, rect2
    | false -> rect2, rect1

  let SortHorizontal (rect1 : Rectangle) (rect2 : Rectangle) =
    match rect1.Right < rect2.Left with
    | true -> rect1, rect2
    | false -> rect2, rect1

  ///Returns success and a axis-aligned corridor between the two rectangles if it exists.
  let Corridor (rect1 : Rectangle) (rect2 : Rectangle) =
    match ShareAxis rect1 rect2 with
    | None -> None
    | Some(ax) ->
      match ax with
      | Axis.Horizontal ->
        let first, second = SortVertical rect1 rect2
        let pos1 = Vector2i(Math.Max(first.Left, second.Left), first.Bottom + 1)

        ///Make corridor of width 2 if possible. But also handles a corridor of one width
        let minX = Math.Min(pos1.X + 1, Math.Min(first.Right, second.Right))

        let pos2 = Vector2i(minX, second.Top - 1)
        Some(Rectangle.Enclose pos1 pos2)
      | Axis.Vertical ->
        let first, second = SortHorizontal rect1 rect2
        let pos1 = Vector2i(first.Right + 1, Math.Max(first.Top, second.Top))

        ///Make corridor of heighth 2 if possible. But also handles a corridor of one height
        let minY = Math.Min(pos1.Y + 1, Math.Min(first.Bottom, second.Bottom))

        let pos2 = Vector2i(second.Left - 1, minY)
        Some(Rectangle.Enclose pos1 pos2)

  let Generate rngSeed =
    let rng = RNG.SeedRNG rngSeed
    //Hardcoded dungeon size
    let dungeon_size = Rectangle(Vector2i(561, 354))
    //Empty map
    let dungeon_map = DungeonMap(dungeon_size)
    //Room params
    let min_room_size = MinimumRoomArea + 5
    let max_room_size = Vector2i(61, 61)
    //BSP rooms with randomized size (Rectangle List).
    let dungeonRooms =
      BspGenerator(min_room_size, max_room_size, dungeon_size.Size).GenerateRoomDivides rng
      |> List.map (RandomizeRoom rng)
    //Feed the randomized rooms to the map
    dungeonRooms |> List.iter (GenerateRoom dungeon_map)
    ///For each room, tests it against every room after it
    let rec CreateRoomCorridors(L : list<Rectangle>) =
      match L with
      | head :: tail ->
        tail
        |> List.choose (Corridor head)
        |> List.append (CreateRoomCorridors tail)
      | [] -> []

    ///Tests for collisions with rooms
    let Collides(corr : Rectangle) = dungeonRooms |> List.exists (corr.Intersects)

    let dungeonCorridors =
      dungeonRooms
      |> CreateRoomCorridors
      |> List.filter (Collides >> not)

    //Draw the corridors on the map
    dungeonCorridors |> List.iter (GenerateCorridor dungeon_map)
    //Return a chunk to feed to visualizer
    dungeon_map.CreateChunk()
