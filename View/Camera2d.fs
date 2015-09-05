namespace Morgemil.View

open Microsoft.Xna.Framework
open Morgemil.Math

/// The spriteBatch draw transform
type Camera2d(Zoom : float32, Position : Vector2) = 
  
  ///Apply this to SpriteBatch Draw
  member this.Matrix = Matrix.CreateTranslation(-Position.X, -Position.Y, 0.0f) * Matrix.CreateScale(new Vector3(Zoom))
  
  member this.Zoom = Zoom
  member this.Position = Position
  
  ///Add this to the Position
  member this.Move(direction : Vector2) = Camera2d(Zoom, Position + direction)
  
  ///Adds to the current zoom
  member this.AddZoom(zoomLvl : float32) = Camera2d(Zoom + zoomLvl, Position)
  
  ///Sets the zoom directly
  member this.SetZoom(zoomLvl : float32) = Camera2d(zoomLvl, Position)
  
  ///Adds the rotation in radians
  member this.Rotate(rot : float32) = Camera2d(Zoom, Position)
  
  ///Sets the rotation directly
  member this.SetRotate(rot : float32) = Camera2d(Zoom, Position)
  
  ///1px tiles. position (0,0)
  static member Default = Camera2d(1.0f, Vector2.Zero)
  
  ///Centers the camera and gives each tile a (zoom,zoom) pixels
  static member CenterCamera zoom (tileLocation : Vector2i) = 
    Camera2d(zoom, new Vector2(float32 (tileLocation.X), float32 (tileLocation.Y)))
  
  ///Zooms out to show the entire map
  static member ShowMap (level : Morgemil.Map.Level) (screenSize : Vector2i) = 
    Camera2d
      (System.Math.Min
         (float32 (screenSize.X) / float32 (level.Area.Width), float32 (screenSize.Y) / float32 (level.Area.Height)), 
       new Vector2(float32 (float (level.Area.Width) / 2.0), float32 (float (level.Area.Height) / 2.0)))
