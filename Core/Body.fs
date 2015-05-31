namespace Morgemil.Game

///A body with base stats/characteristics.
///Any mutable data is in a higher level.
type Body =
  { Id : int
    Race : Race
    ///Normal "bodies" fit in one tile (1,1). Bosses and the largest entities can take up multiple tiles.
    Size : Morgemil.Math.Vector2i }
