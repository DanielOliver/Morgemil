namespace Morgemil.Game

open Morgemil.Math

///This is a high level view of an entity. Typically holds any mutable data (can change each game step).
type PersonDefinition = 
  { Id : int
    Race : RaceDefinition
    Position : Vector2i }
  member this.Area = Rectangle(this.Position, this.Race.Size)
