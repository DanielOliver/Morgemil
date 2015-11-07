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
  | EmptyTrigger of EntityId : EntityId * Data : EmptyTrigger * TriggerId : TriggerId
  | SomeTrigger of EntityId : EntityId * Data : SomeTrigger * TriggerId : TriggerId
  
  member this.Id = 
    match this with
    | EmptyTrigger(_, _, id) -> id
    | SomeTrigger(_, _, id) -> id
  
  member this.Entity = 
    match this with
    | EmptyTrigger(id, _, _) -> id
    | SomeTrigger(id, _, _) -> id
