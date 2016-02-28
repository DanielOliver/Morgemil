namespace Morgemil.Core

open Morgemil.Math

///This is a high level view of an entity. Typically holds any mutable data (can change each game step).
type Person = 
  { Id : int
    Race : Race }
