namespace Morgemil.Map

open Morgemil.Math

///Creates a Dungeon
module DungeonGeneration =
  /// The existence of a mutable structure is necessary as a dungeon requires multiple passes to generate
  type private DungeonMap(roomSize : Rectangle) =
    let internal_map = Array.create roomSize.Area (TileDefinition.Default)
    member this.SetValue pos tile = internal_map.[roomSize.FlatCoord pos] <- tile
    ///Return a single giant chunk to feed into the Visualizer
    member this.CreateChunk() = Chunk(roomSize, internal_map)

  ///The absolute minimum room area tolerated
  let private MinimumRoomArea = Rectangle(Vector2i(33, 33))

  ///Randomizes a room's position and size within the defined area
  let private RandomizeRoom rng (maxArea : Rectangle) =
    //Randomize size before placing the room randomly
    let room_size =
      MinimumRoomArea.Size + RNG.RandomVector rng (maxArea.Size - MinimumRoomArea.Size)
    let room_position = maxArea.Position + RNG.RandomVector rng (maxArea.Size - room_size)
    Rectangle(room_position, room_size)

  ///Writes directly on the map (side-effects). returns nothing
  let private GenerateRoom (dungeonMap : DungeonMap) (area : Rectangle) =
    let ChooseTile pt =
      match pt with
      | _ when area.IsOnEdge(pt) -> Tiles.DungeonWall
      | _ -> Tiles.DungeonFloor
    area.Coordinates |> Seq.iter (fun coord -> dungeonMap.SetValue coord (ChooseTile coord))

  let Generate rngSeed =
    let rng = RNG.SeedRNG rngSeed
    //Hardcoded dungeon size
    let dungeon_size = Rectangle(Vector2i(512, 512))
    //Empty map
    let dungeon_map = DungeonMap(dungeon_size)
    //Room params
    let min_room_size = Vector2i(35, 35)
    let max_room_size = Vector2i(61, 61)
    //BSP rooms with randomized size (Rectangle List).
    let rooms =
      BspGenerator(min_room_size, max_room_size, dungeon_size.Size).GenerateRoomDivides rng
      |> List.map (RandomizeRoom rng)
    //Feed the randomized rooms to the map
    rooms |> List.iter (GenerateRoom dungeon_map)
    //Return a chunk to feed to visualizer
    dungeon_map.CreateChunk()
