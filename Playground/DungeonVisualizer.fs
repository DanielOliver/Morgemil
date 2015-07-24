namespace Morgemil.Test

open System.Drawing

module DungeonVisualizer = 
  open Morgemil.Map
  open Morgemil.Math
  
  let DrawPlayer (player : Morgemil.Game.Person) (image : Bitmap) = 
    image.SetPixel(player.Position.X, player.Position.Y, Color.BlueViolet)
  
  ///Draws chunks to the minimum image space necessary
  let Visualize(chunks : seq<Chunk>) = 
    let minimumImageSize = 
      chunks
      |> Seq.map (fun chun -> chun.Area)
      |> Seq.reduce (+)
    
    ///Not a true FP structure. So modify at will.
    let resultingImage = new Bitmap(minimumImageSize.Width, minimumImageSize.Height)
    
    let DrawTile(tile : TileDefinition, pos : Vector2i) = 
      let tileColor = 
        match tile with
        | _ when Tiles.DungeonFloor.ID = tile.ID -> Color.White
        | _ when Tiles.DungeonCorridor.ID = tile.ID -> Color.Red
        | _ -> Color.Black
      resultingImage.SetPixel(pos.X, pos.Y, tileColor)
    
    let DrawChunk(chunk : Chunk) = 
      chunk.Area.Coordinates
      |> Seq.map (fun xy -> (chunk.Tile xy, xy - minimumImageSize.MinCoord))
      |> Seq.iter (DrawTile)
    
    chunks |> Seq.iter (DrawChunk)
    resultingImage
