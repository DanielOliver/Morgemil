namespace Morgemil.Core

type PositionComponent = 
  { Entity : Entity
    Position : Vector2i
    Mobile : bool }

type ControlComponent = 
  { Entity : Entity
    HumanControlled : bool }

type Components = 
  | Position of PositionComponent
  | Control of ControlComponent
