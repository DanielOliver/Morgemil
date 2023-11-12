namespace Morgemil.GameEngine

open Morgemil.Core
open Morgemil.Math
open SadConsole

type CrawlSidebar(width: int, height: int, xOffset: int, initialGameData: InitialGameData, loopContext: LoopContext) as this
    =
    inherit SadConsole.Console(width, height)

    do
        base.Position <- Point(xOffset, 0)
        base.Surface.DefaultBackground <- Color.PaleVioletRed
        this.Reposition()

    member this.Reposition() =
        let calculated = SadConsoleExtensions.SadConsoleCellsOnScreen()
        let targetArea = Rectangle(calculated.X - 20, 1, 19, calculated.Y - 2)
        base.Position <- targetArea.MinExtent
        base.Resize(targetArea.Width, targetArea.Height, true)

    override this.Render(delta: System.TimeSpan) =
        base.Render(delta)
        base.Clear()

        base.Print(0, 0, ColoredString("-- " + initialGameData.Scenario.Name + " --", Color.White, Color.Black))

        base.Print(
            0,
            1,
            ColoredString(
                "-- Floor " + loopContext.GameContext.Value.FloorID.Key.ToString() + " --",
                Color.White,
                Color.Black
            )
        )

        let mutable index = 2

        for character in loopContext.Characters |> Table.Items do
            let attributes = loopContext.CharacterAttributes.ByID character.ID

            if character.PlayerID.IsSome then
                base.Print(0, index, ColoredString("@ " + attributes.Ancestry.Noun, Color.Black, Color.White))
            else
                base.Print(0, index, ColoredString("M " + attributes.Ancestry.Noun, Color.Black, Color.Transparent))

            index <- index + 1
