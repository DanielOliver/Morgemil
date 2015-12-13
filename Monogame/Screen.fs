namespace Morgemil.Monogame

open Morgemil.Core

type ScreenAnchor = 
  | Left
  | Right
  | Top
  | Bottom
  | None

type ScreenSize = 
  | Pixels of int
  | Ratio of float
  
  member this.ToPixel x = 
    match this with
    | ScreenSize.Pixels(p) -> p
    | ScreenSize.Ratio(r) -> int (System.Math.Floor(r * float (x)))
  
  static member Zero = ScreenSize.Ratio 0.0

type ScreenVector2 = 
  { X : ScreenSize
    Y : ScreenSize }
  member this.ToVector2i(actualScreenSize : Vector2i) = 
    Vector2i(this.X.ToPixel(actualScreenSize.X), this.Y.ToPixel(actualScreenSize.Y))
  static member Zero = 
    { X = ScreenSize.Zero
      Y = ScreenSize.Zero }


type ScreenRectangle = 
  { Position : ScreenVector2
    Size : ScreenVector2 }
  member this.ToRectangle(actualScreenSize : Vector2i) = 
    Rectangle(this.Position.ToVector2i(actualScreenSize), this.Size.ToVector2i(actualScreenSize))
  member this.Intersects (actualScreenSize : Vector2i) (rect : ScreenRectangle) = 
    this.ToRectangle(actualScreenSize).Intersects(rect.ToRectangle(actualScreenSize))
  static member FullScreen = 
    { Position = ScreenVector2.Zero
      Size = 
        { ScreenVector2.X = ScreenSize.Ratio(1.0)
          Y = ScreenSize.Ratio(1.0) } }
