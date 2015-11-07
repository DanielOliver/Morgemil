namespace Morgemil.Core

type TriggerId = 
  | TriggerId of int
  member this.Value = 
    match this with
    | TriggerId(id) -> id

type EmptyTrigger = 
  { Name : string }

type SomeTrigger = 
  { Name : string }

type Trigger = 
  | Empty of EntityId : EntityId * Data : EmptyTrigger * TriggerId : TriggerId
  | SomeT of EntityId : EntityId * Data : SomeTrigger * TriggerId : TriggerId
  
  member this.Id = 
    match this with
    | Empty(_, _, id) -> id
    | SomeT(_, _, id) -> id
  
  member this.Entity = 
    match this with
    | Empty(id, _, _) -> id
    | SomeT(id, _, _) -> id
