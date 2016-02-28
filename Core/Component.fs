namespace Morgemil.Core

open Morgemil.Math

type ComponentType = 
  | Position
  | Player
  | Resouce
  | Action

type PositionComponent = 
  { EntityId : EntityId
    Position : Vector2i
    Mobile : bool }
  member this.ToComponent() = Component.Position(this)

and PlayerComponent = 
  { EntityId : EntityId
    IsHumanControlled : bool }
  member this.ToComponent() = Component.Player(this)

and ResourceComponent = 
  { EntityId : EntityId
    Stamina : float<Stamina> }
  member this.ToComponent() = Component.Resource(this)

and ActionComponent = 
  { EntityId : EntityId
    TimeOfRequest : float<GameTime>
    TimeOfNextAction : float<GameTime> }
  member this.Duration = this.TimeOfNextAction - this.TimeOfRequest
  member this.ToComponent() = Component.Action(this)

and Component = 
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
  
  member this.Type = 
    match this with
    | Component.Position(_) -> ComponentType.Position
    | Component.Player(_) -> ComponentType.Player
    | Component.Resource(_) -> ComponentType.Resouce
    | Component.Action(_) -> ComponentType.Action
