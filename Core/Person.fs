namespace Morgemil.Game

///This is a high level view of an entity. Typically holds any mutable data (can change each game step).
type Person =
  { Id : int
    //Body : Body
    ///This plus Body.Size constructs the person's hitbox
    Position : Morgemil.Math.Vector2i }
