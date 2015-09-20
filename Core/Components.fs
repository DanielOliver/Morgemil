namespace Morgemil.Core

type PositionComponent = 
  { Entity : Entity
    Position : Vector2i
    Mobile : bool }

type ControlComponent = 
  { Entity : Entity
    IsHumanControlled : bool }

type ResourceComponent = 
  { Entity : Entity
    ResourceAmount : double }

type Components = 
  | Position of PositionComponent
  | Control of ControlComponent
  | Resource of ResourceComponent
