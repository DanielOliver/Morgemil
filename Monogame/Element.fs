namespace Morgemil.Monogame

type ElementType = 
  | Textbox
  | Button
  | Root

type Element = 
  { Anchor: ScreenAnchor
    Position: ScreenVector2
    Size: ScreenVector2
   }


