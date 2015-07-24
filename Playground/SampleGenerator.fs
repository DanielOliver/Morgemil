namespace Morgemil.Test

open Morgemil.Map
open Morgemil.Math

module SimpleDungeon = 
  //This is essentially a constant since it takes no arguments
  let DungeonGenerator = 
    let dungeonLength = 10 ///Lets make 3 rooms.
    
    let dungeonFloor = TileDefinition(1, "Floor", "Dungeon floors are often trapped", false, true, TileType.Land)
    
    let chunkSize = 90 //A chunk will be 10x10 square
    
    ///(0,0) (90,0) (180,0)...
    let chunksToCreatePositions = { 0..(dungeonLength - 1) } |> Seq.map (fun x -> new Vector2i(chunkSize * x, 0))
    
    ///Given the corner of the chunk to create
    let CreateRoomChunk(chunkPosition : Vector2i) = 
      let roomArea = Rectangle(chunkPosition, Vector2i(chunkSize, chunkSize))
      
      ///border is empty tiles and the contained area is dungeon floor
      let ChooseTile(tilePosition : Vector2i) = 
        match tilePosition with
        | _ when tilePosition.X = roomArea.Left || tilePosition.X = roomArea.Right || tilePosition.Y = roomArea.Top 
                 || tilePosition.Y = roomArea.Bottom -> TileDefinition.Default
        | _ -> dungeonFloor
      
      ///Maps each coordinate in the room into a tile
      let tileArray = 
        roomArea.Coordinates
        |> Seq.map (ChooseTile)
        |> Seq.toArray
      
      Chunk(roomArea, tileArray)
    
    chunksToCreatePositions |> Seq.map (CreateRoomChunk)
