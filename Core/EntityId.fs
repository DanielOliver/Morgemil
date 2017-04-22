namespace Morgemil.Core

[<Struct>]
type EntityId = 
  | EntityId of int
  
  member this.Value = 
    match this with
    | EntityId(id) -> id
