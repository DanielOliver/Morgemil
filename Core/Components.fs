namespace Morgemil.Core

type PositionComponent = 
  { Entity : Entity
    Position : Vector2i
    Mobile : bool }

type ControllerComponent = 
  { Entity : Entity
    IsHumanControlled : bool }

type ResourceComponent = 
  { Entity : Entity
    ResourceAmount : double }

type Components = 
  | Position of PositionComponent
  | Control of ControllerComponent
  | Resource of ResourceComponent
