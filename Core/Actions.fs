namespace Morgemil.Core

open Morgemil.Math

///The actions available to an entity.
type Actions = 
  | Movement of Direction : Vector2i
