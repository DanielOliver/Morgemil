namespace Morgemil.Logic

open Morgemil.Core

type TriggerBuilder(noAction : Trigger) = 
  let mutable results = TurnStep.Empty
  member this.Bind(x, f) = f x
  member this.Zero() = TriggerStatus.NoAction(noAction)
  
  member this.Yield(expr : EventResult) : TurnStep = 
    results <- (expr :: results)
    [ expr ]
  
  member this.Return(expr : TriggerStatus) = expr
  member this.Return(expr : TurnStep -> TriggerStatus) = expr results
  member this.Return(expr : Trigger * TurnStep -> TriggerStatus) = expr (noAction, results)
  member this.Yield(expr) = ()
  member this.Combine(a : TurnStep, b : TriggerStatus) = b
  member this.Delay(f) = f()
