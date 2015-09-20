namespace Morgemil.Core

type EntityType = 
  | Person
  | Door
  | Stairs
  | Object

type Entity = 
  { Id : int
    Type : EntityType }
