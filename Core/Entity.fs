namespace Morgemil.Core

type EntityId = EntityId of int

type EntityType = 
  | Person
  | Door
  | Stairs
  | Object

type Entity = 
  { Id : EntityId
    Type : EntityType }
