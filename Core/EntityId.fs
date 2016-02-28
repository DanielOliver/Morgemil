namespace Morgemil.Core

type EntityId = 
  | EntityId of int
  
  member this.Value = 
    match this with
    | EntityId(id) -> id
  
  static member IsDefault(x : EntityId) = x.Value < 0

type EntityType = 
  | Person
  | Door
  | Stairs
  | Object
  | Spell
