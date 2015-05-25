namespace Morgemil.Map

open Morgemil.Math

/// <summary>
/// The existence of a mutable structure is necessary as a dungeon requires multiple passes to generate
/// </summary>
type private DungeonMap(roomSize : Rectangle) =
  let internal_map = Array.create roomSize.Area (TileDefinition.Default)
  member this.SetValue pos tile = internal_map.[roomSize.FlatCoord pos] <- tile
  member this.CreateChunk() = Chunk(roomSize, internal_map)

///Creates a Dungeon
module DungeonGeneration =
  let private MinimumRoomArea = Rectangle(Vector2i(33, 33))

  let private RandomizeRoom rng (maxArea : Rectangle) =
    let room_size =
      MinimumRoomArea.Size + RNG.RandomVector rng (maxArea.Size - MinimumRoomArea.Size)
    let room_position = maxArea.Position + RNG.RandomVector rng (maxArea.Size - room_size)
    Rectangle(room_position, room_size)

  ///Writes directly on the map (side-effects)
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
    let room_list =
      BspGenerator.GenerateRoomDivides rng dungeon_size.Size |> List.map (RandomizeRoom rng)
    let dungeon_map = DungeonMap(dungeon_size)
    room_list |> List.iter (GenerateRoom dungeon_map)
    dungeon_map.CreateChunk()
