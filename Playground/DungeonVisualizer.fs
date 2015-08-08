namespace Morgemil.Test

open System.Drawing

module DungeonVisualizer = 
  open Morgemil.Map
  open Morgemil.Math
  
  let DrawPlayer (player : Morgemil.Game.PersonDefinition) (image : Bitmap) = 
    image.SetPixel(player.Position.X, player.Position.Y, Color.BlueViolet)
  
  ///Draws a level to the minimum image space necessary
  let Visualize(level : LevelDefinition) = 
    let minimumImageSize = level.Area
    
    ///Not a true FP structure. So modify at will.
    let resultingImage = new Bitmap(minimumImageSize.Width, minimumImageSize.Height)
    
    let DrawTile(tile : TileDefinition, pos : Vector2i) = 
      let tileColor = 
        match tile with
        | _ when Tiles.DungeonFloor.Id = tile.Id -> Color.White
        | _ when Tiles.DungeonCorridor.Id = tile.Id -> Color.Red
        | _ -> Color.Black
      resultingImage.SetPixel(pos.X, pos.Y, tileColor)
    
    level.Area.Coordinates
    |> Seq.map (fun xy -> (level.Tile xy, xy - minimumImageSize.MinCoord))
    |> Seq.iter (DrawTile)
    resultingImage
