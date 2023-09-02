module Morgemil.GameEngine.SadConsoleExtensions

open SadRogue.Primitives

let SadConsoleCellsOnScreen () : Point =
    let width =
        SadConsole.Game.Instance.MonoGameInstance.WindowWidth
        / SadConsole.Game.Instance.DefaultFont.GlyphWidth

    let height =
        SadConsole.Game.Instance.MonoGameInstance.WindowHeight
        / SadConsole.Game.Instance.DefaultFont.GlyphHeight

    Point(width, height)
