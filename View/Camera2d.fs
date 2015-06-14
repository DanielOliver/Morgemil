namespace Morgemil.View

open Microsoft.Xna.Framework

/// The spriteBatch draw transform
type Camera2d(Zoom : float32, Position : Vector2) =

  ///Apply this to SpriteBatch Draw
  member this.Matrix =
    Matrix.CreateTranslation(Position.X, Position.Y, 0.0f) * Matrix.CreateScale(Zoom)

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

  //1px tiles. position (0,0)
  static member Default = Camera2d(1.0f, Vector2.Zero)
