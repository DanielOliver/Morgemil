namespace Morgemil.Core

type EntityId = int

type EntityType = 
  | Person
  | Door
  | Stairs
  | Object

type Entity = 
  { Id : EntityId
    Type : EntityType }
