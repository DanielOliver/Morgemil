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

type ActionComponent = 
  { EntityId : EntityId
    TimeOfNextAction : float<GameTime> }

type Component = 
  | Position of PositionComponent
  | Player of PlayerComponent
  | Resource of ResourceComponent
  | Action of ActionComponent
  member this.EntityId = 
    match this with
    | Component.Position(x) -> x.EntityId
    | Component.Player(x) -> x.EntityId
    | Component.Resource(x) -> x.EntityId
    | Component.Action(x) -> x.EntityId
