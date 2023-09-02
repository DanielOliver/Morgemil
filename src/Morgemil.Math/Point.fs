namespace Morgemil.Math

type Point = SadRogue.Primitives.Point

module Point =
    let inline create (x, y) = Point(x, y)

    let Identity = Point(0, 1)
    let Zero = Point.Zero
