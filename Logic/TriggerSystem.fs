namespace Morgemil.Logic

open Morgemil.Core

type TriggerStatus = 
  | Done of TurnStep
  | Continue of Trigger * TurnStep

type TriggerSystem(initial, triggers) = 
  inherit ComponentSystem<TriggersComponent>(initial, (fun position -> position.EntityId))
  let _triggers : Map<TriggerId, Trigger> = triggers //TODO: Map triggers to ID
  static member Empty = TriggerSystem(Seq.empty, Map.empty)
