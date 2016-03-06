namespace Morgemil.View

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Morgemil.Core
open Morgemil.Math
open SadConsole
open SadConsole.Consoles

type SplashConsole() as this = 
  inherit Console(80, 25)
  let mountains = System.IO.File.ReadAllLines("mountains.txt")
  
  let TextImage(lines : seq<string>) = 
    (lines, 
     Vector2i(lines
                   |> Seq.map (fun t -> t.Length)
                   |> Seq.max, lines |> Seq.length))
  
  let GetSize() = Vector2i(this.CellData.Width, this.CellData.Height)
  
  ///Draws text on the center of the screen
  let DrawCenter(text) = 
    let windowSize = GetSize()
    let (_, textSize) = TextImage(text)
    let drawRectangle = Rectangle((windowSize - textSize) / 2, textSize)
    
    let xOffset = 
      if drawRectangle.Left < 0 then 0 - drawRectangle.Left
      else 0
    for y in drawRectangle.Top..drawRectangle.Bottom do
      if y >= 0 && y < windowSize.Y then 
        let line = mountains.[y - drawRectangle.Top]
        let stringLength = System.Math.Min(line.Length, windowSize.X - System.Math.Max(drawRectangle.Left, 0))
        this.CellData.Print(drawRectangle.Left + xOffset, y, line.Substring(xOffset, stringLength), Color.White)
    drawRectangle
  
  let DrawColors() = 
    let sizeY = GetSize().Y
    let relative (y, scale) = int (float (scale) * float (y) / float (sizeY))
    let maxRed = 120
    this.CellData |> Seq.iter (fun t -> t.Background <- new Color(maxRed - relative (t.Position.Y, maxRed), 0, 0))
  
  let startText = "Press (Space) to start"
  let startX = (GetSize().X / 2) - (startText.Length / 2)
  do this.CellData.Print(startX, 24, startText)
  let effect = new SadConsole.Effects.Blink()
  
  do 
    effect.BlinkSpeed <- 0.5
    for x in 0..(startText.Length - 1) do
      this.CellData.SetEffect(this.CellData.Item(x + startX, 24), effect)
  
  member this.Mountains = mountains
  override this.Render() = 
    //    this.CellData.Clear()
    DrawColors()
    let drawRectangle = DrawCenter(mountains)
    base.Render()
