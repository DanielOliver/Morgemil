namespace Morgemil.Core

type PositionComponent = 
  { EntityId : EntityId
    Position : Vector2i
    Mobile : bool }

type PlayerComponent = 
  { EntityId : EntityId
    IsHumanControlled : bool }

type ResourceComponent = 
  { EntityId : EntityId
    ResourceAmount : double }

type Components = 
  | Position of PositionComponent
  | Player of PlayerComponent
  | Resource of ResourceComponent
  member this.EntityId = 
    match this with
    | Components.Position(x) -> x.EntityId
    | Components.Player(x) -> x.EntityId
    | Components.Resource(x) -> x.EntityId
