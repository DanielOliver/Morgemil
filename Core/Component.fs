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
    TimeOfRequest : float<GameTime>
    TimeOfNextAction : float<GameTime> }
  member this.Duration = this.TimeOfNextAction - this.TimeOfRequest

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

[<AbstractClass>]
type ComponentAggregator(entityId : EntityId) = 
  member this.EntityId = entityId
  abstract Position : PositionComponent option
  abstract Player : PlayerComponent option
  abstract Resource : ResourceComponent option
  abstract Action : ActionComponent option
  abstract Triggers : seq<Trigger>
