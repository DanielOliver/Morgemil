namespace Morgemil.Math

type Color = SadRogue.Primitives.Color

module Color =
    let From (r: int, g: int, b: int, a: int) = Color(r, g, b, a)

    let blendColors (color1: Color) (color2: Color) : Color =
        if color1.A = System.Byte.MaxValue || color2.A = System.Byte.MinValue then
            color1
        elif color1.A = System.Byte.MinValue then
            color2
        else
            let ratio = (float32 color2.A) / (float32 color1.A + float32 color2.A)
            let returnColor = SadRogue.Primitives.Color.Lerp(color1, color2, ratio)
            SadRogue.Primitives.Color(returnColor, 255)
