namespace Morgemil.Test

open System.Drawing

module DungeonVisualizer =
  open Morgemil.Map
  open Morgemil.Math

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
        | _ when TileDefinition.IsDefault tile -> Color.Black
        | _ when Tiles.DungeonFloor.ID = tile.ID -> Color.White
        | _ when Tiles.DungeonWall.ID = tile.ID -> Color.Red
        | _ -> Color.Gray
      resultingImage.SetPixel(pos.X, pos.Y, tileColor)

    let DrawChunk(chunk : Chunk) =
      chunk.Area.Coordinates
      |> Seq.map (fun xy -> (chunk.Tile xy, xy - minimumImageSize.MinCoord))
      |> Seq.iter (DrawTile)

    chunks |> Seq.iter (DrawChunk)
    resultingImage
