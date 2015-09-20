namespace Morgemil.Logic

type TurnStep = 
  { Requests : List<EventRequest>
    Results : List<EventResult> }
  
  member this.Combine(other : TurnStep) = 
    { Requests = List.concat [ this.Requests; other.Requests ]
      Results = List.concat [ this.Results; other.Results ] }
  
  static member Empty = 
    { Requests = List.empty
      Results = List.empty }

type TurnBuilder() = 
  member this.Bind(x, f) = f x
  member this.Zero() = TurnStep.Empty
  
  member this.Yield(expr : EventResult) = 
    { Requests = List.empty
      Results = [ expr ] }
  
  member this.Yield(expr : EventRequest) = 
    { Requests = [ expr ]
      Results = List.empty }
  
  member this.Return(expr) = TurnStep.Empty
  member this.Yield(expr) = TurnStep.Empty
  member this.Combine(a : TurnStep, b : TurnStep) = a.Combine b
  member this.Delay(f) = f()
