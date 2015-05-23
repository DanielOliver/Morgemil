namespace Morgemil.Test

module SimpleDungeon =
  //This is essentially a constant since it takes no arguments
  let DungeonGenerator =
    let dungeonLength = 3 //Lets make 3 rooms.
    let dungeonFloor =
      Morgemil.Map.TileDefinition
        (1, "Floor", "Dungeon floors are often trapped", false, true, Morgemil.Map.TileType.Land)
    let chunkSize = 10 //A chunk will be 10x10 square

    ///(0,0) (10,0) (20,0)...
    let chunksToCreatePositions =
      { 0..(dungeonLength - 1) } |> Seq.map (fun x -> new Morgemil.Math.Vector2i(chunkSize * x, 0))

    //Given the corner of the chunk to create
    let CreateRoomChunk(chunkPosition : Morgemil.Math.Vector2i) =
      let roomArea =
        Morgemil.Math.Rectangle(chunkPosition, Morgemil.Math.Vector2i(chunkSize, chunkSize))

      //border is empty tiles and the contained area is dungeon floor
      let ChooseTile(tilePosition : Morgemil.Math.Vector2i) =
        match tilePosition with
        | _ when tilePosition.X = roomArea.Left || tilePosition.X = roomArea.Right
                 || tilePosition.Y = roomArea.Top || tilePosition.Y = roomArea.Bottom ->
          Morgemil.Map.TileDefinition.Default
        | _ -> dungeonFloor

      //Maps each coordinate in the room into a tile
      let tileArray =
        roomArea.Coordinates
        |> Seq.map (ChooseTile)
        |> Seq.toArray

      Morgemil.Map.Chunk(roomArea, tileArray)

    chunksToCreatePositions |> Seq.map (CreateRoomChunk)
