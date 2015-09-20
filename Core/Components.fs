namespace Morgemil.Core

type PositionComponent = 
  { Entity : Entity
    Position : Vector2i
    Mobile : bool }

type PlayerComponent = 
  { Entity : Entity
    IsHumanControlled : bool }

type ResourceComponent = 
  { Entity : Entity
    ResourceAmount : double }

type Components = 
  | Position of PositionComponent
  | Player of PlayerComponent
  | Resource of ResourceComponent
  member this.EntityId = 
    match this with
    | Components.Position(x) -> x.Entity.Id
    | Components.Player(x) -> x.Entity.Id
    | Components.Resource(x) -> x.Entity.Id
