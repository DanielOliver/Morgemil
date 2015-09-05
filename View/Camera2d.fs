namespace Morgemil.View

open Microsoft.Xna.Framework
open Morgemil.Math

/// The spriteBatch draw transform
type Camera2d(zoom : float32, position : Vector2, graphics : GraphicsDeviceManager, level : Morgemil.Map.Level) = 
  let self _zoom _position = Camera2d(_zoom, _position, graphics, level)
  
  ///Apply this to SpriteBatch Draw
  member this.Matrix = 
    Matrix.CreateTranslation(-position.X, -position.Y, 0.0f) * Matrix.CreateScale(new Vector3(zoom)) 
    * Matrix.CreateTranslation
        (float32 (graphics.PreferredBackBufferWidth / 2), float32 (graphics.PreferredBackBufferHeight / 2), 0.0f)
  
  ///Used to convert screen to world
  member this.InvertMatrix = Matrix.Invert(this.Matrix)
  
  ///Screen to world coordinates
  member this.ScreenCoordinatesToWorld(screenCoord : Vector2) = 
    let coord = Vector2.Transform(screenCoord, this.InvertMatrix)
    Vector2i(int (coord.X), int (coord.Y))
  
  member this.Zoom = zoom
  member this.Position = position
  
  ///Add this to the position
  member this.Move(direction : Vector2) = self zoom (position + direction)
  
  ///Adds to the current zoom
  member this.Addzoom(zoomLvl : float32) = self (zoom + zoomLvl) position
  
  ///Sets the zoom directly
  member this.Setzoom(zoomLvl : float32) = self zoomLvl position
  
  ///1px tiles. position (0,0)
  static member Default (graphics : GraphicsDeviceManager) (level : Morgemil.Map.Level) = 
    Camera2d(1.0f, Vector2.Zero, graphics, level)
  
  ///Centers the camera and gives each tile a (zoom,zoom) pixels
  member this.CenterCamera(zoom, (tileLocation : Vector2i)) = 
    self zoom (new Vector2(float32 (tileLocation.X), float32 (tileLocation.Y)))
  
  ///Centers the camera and gives each tile a (zoom,zoom) pixels
  member this.CenterCamera tileLocation = this.CenterCamera(zoom, tileLocation)
  
  ///zooms out to show the entire map
  member this.ShowMap() = 
    self 
      (System.Math.Min
         (float32 (graphics.PreferredBackBufferWidth) / float32 (level.Area.Width), 
          float32 (graphics.PreferredBackBufferHeight) / float32 (level.Area.Height))) 
      (new Vector2(float32 (float (level.Area.Width) / 2.0), float32 (float (level.Area.Height) / 2.0)))
