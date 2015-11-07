﻿namespace Morgemil.Logic

open Morgemil.Core

type TriggerStatus = 
  | Done of TurnStep
  | Continue of Trigger * TurnStep
  | NoAction of Trigger

type TriggerAction = Trigger -> TriggerStatus

type TriggerSystem(triggers : seq<Trigger>) = 
  
  let _identity = 
    IdentityPool(triggers
                 |> Seq.map (fun t -> t.Id.Value)
                 |> Set.ofSeq)
  
  let mutable _triggers : Map<TriggerId, Trigger> = 
    triggers
    |> Seq.map (fun t -> t.Id, t)
    |> Map.ofSeq
  
  member this.Items = _triggers |> Seq.map (fun t -> t.Value)
  
  member this.Add(create : TriggerId -> Trigger) = 
    let gen = TriggerId(_identity.Generate())
    let result = create gen
    _triggers <- _triggers.Add(gen, result)
    result
  
  member this.Remove triggerId = 
    _triggers <- _triggers.Remove(triggerId)
    _identity.Free triggerId.Value
  
  member this.Handle(action : TriggerAction) : TurnStep = 
    let results = _triggers |> Map.map (fun _ value -> action value)
    //Filters out the remaining triggers
    _triggers <- results
                 |> Seq.choose (fun trigger -> 
                      match trigger.Value with
                      | TriggerStatus.Continue(x, _) -> Some(x.Id, x)
                      | TriggerStatus.NoAction(x) -> Some(x.Id, x)
                      | _ -> 
                        this.Remove(trigger.Key)
                        None)
                 |> Map.ofSeq
    //Creates the result set
    results
    |> Seq.choose (fun trigger -> 
         match trigger.Value with
         | TriggerStatus.Continue(_, x) -> Some(x)
         | TriggerStatus.Done(x) -> Some(x)
         | _ -> None)
    |> Seq.fold (fun state t -> List.concat ([ state; t ])) TurnStep.Empty
  
  static member Empty = TriggerSystem(Seq.empty)
