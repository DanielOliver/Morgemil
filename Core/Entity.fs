namespace Morgemil.Core

type EntityId = 
  | EntityId of int
  member this.Value = 
    match this with
    | EntityId(id) -> id

type EntityType = 
  | Person
  | Door
  | Stairs
  | Object
  | Spell

type Entity = 
  { Id : EntityId
    Type : EntityType }
