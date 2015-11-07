namespace Morgemil.Logic

type TurnBuilder() = 
  member this.Bind(x, f) = f x
  member this.Zero() = TurnStep.Empty
  member this.Yield(expr : EventResult) : TurnStep = [ expr ]
  member this.Return(expr) = TurnStep.Empty
  member this.Yield(expr) = TurnStep.Empty
  member this.Combine(a : TurnStep, b : TurnStep) = List.concat [ a; b ]
  member this.Delay(f) = f()
