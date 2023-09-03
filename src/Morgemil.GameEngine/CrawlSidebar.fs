namespace Morgemil.GameEngine

open Morgemil.Math
open SadConsole

type CrawlSidebar(width, height, xOffset) as this =
    inherit SadConsole.Console(width, height)

    do
        base.Position <- Point(xOffset, 0)
        base.DefaultBackground <- Color.PaleVioletRed
        this.Reposition()

    member this.Reposition() =
        let calculated = SadConsoleExtensions.SadConsoleCellsOnScreen()
        let targetArea = Rectangle(calculated.X - 20, 1, 19, calculated.Y - 2)
        base.Position <- targetArea.MinExtent
        base.Resize(targetArea.Width, targetArea.Height, true)

        base.DrawBox(
            base.Area,
            ShapeParameters.CreateStyledBox(
                ICellSurface.ConnectedLineThin,
                ColoredGlyph(Color.White, Color.TransparentBlack)
            )
        )
