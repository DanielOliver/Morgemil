module Morgemil.Core.Tests.GameStateMachine

open Xunit
open Morgemil.Core
open Morgemil.Models

[<Fact>]
let ``Can transition states``() =
    let exampleLoop(request: ActionRequest): ActionEvent seq =
        Assert.Equal(ActionRequest.Move( Morgemil.Math.Vector2i.Identity), request)
        Seq.empty

    let stateMachine = GameStateMachine exampleLoop
    let currentState = stateMachine.GetCurrentState()
    Assert.Equal(GameState.WaitingForInput, currentState)

    stateMachine.Input( ActionRequest.Move( Morgemil.Math.Vector2i.Identity) )

    let mutable State1 = currentState
    while stateMachine.GetCurrentState() = GameState.Processing do
        State1 <- stateMachine.GetCurrentState()
        System.Threading.Thread.Sleep 500
    let currentState = stateMachine.GetCurrentState()
    Assert.Equal(GameState.Results Seq.empty, currentState)


    stateMachine.Acknowledge()
    let currentState = stateMachine.GetCurrentState()

    
    Assert.Equal(GameState.WaitingForInput, currentState)
