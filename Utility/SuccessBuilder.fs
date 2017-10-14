module Morgemil.Utility.SuccessBuilder

type SuccessBuilder() =
    member this.Bind(m: Result<_,_>, f) = 
        match m with
        | Ok x -> x |> f
        | Error x -> Error x
        
    member this.Return(x) = 
        Ok x
        
    member this.Zero() = 
        Ok()

// make an instance of the workflow 
let success = new SuccessBuilder()
