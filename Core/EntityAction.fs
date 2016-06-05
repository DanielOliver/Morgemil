namespace Morgemil.Core

open Morgemil.Math

///The actions available to an entity.
type EntityAction = 
  | Movement of Direction : Vector2i
  | DownStairs
  | Exit
