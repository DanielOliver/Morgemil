namespace Morgemil.Math


/// Edge-inclusive. Axis-aligned bounding box (AABB)
type Rectangle = SadRogue.Primitives.Rectangle

module Rectangle =
    let inline create (w: int, h: int) : Rectangle = Rectangle(0, 0, w, h)
