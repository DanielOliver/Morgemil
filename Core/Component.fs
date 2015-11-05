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

type TriggersComponent = 
  { EntityId : EntityId
    Triggers : Set<TriggerId> }

type Component = 
  | Position of PositionComponent
  | Player of PlayerComponent
  | Resource of ResourceComponent
  | Triggers of TriggersComponent
  member this.EntityId = 
    match this with
    | Component.Position(x) -> x.EntityId
    | Component.Player(x) -> x.EntityId
    | Component.Resource(x) -> x.EntityId
    | Component.Triggers(x) -> x.EntityId
