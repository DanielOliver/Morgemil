module Morgemil.Core.Tests.GameStateMachine

open Xunit
open Morgemil.Core
open Morgemil.Models

[<Fact>]
let ``Can transition states``() =
    let exampleLoop(request: ActionRequest): ActionEvent seq =
        Assert.Equal(ActionRequest.Move( Morgemil.Math.Vector2i.Identity), request)
        Seq.empty

    let stateMachine: IGameStateMachine = SimpleGameStateMachine exampleLoop :> IGameStateMachine
    Assert.Equal(GameState.WaitingForInput, stateMachine.CurrentState)

    let testState() = 
        while stateMachine.CurrentState = GameState.Processing do
            System.Threading.Thread.Sleep 500
        Assert.Equal(GameState.Results Seq.empty, stateMachine.CurrentState)

    stateMachine.Input( ActionRequest.Move( Morgemil.Math.Vector2i.Identity) )
    testState()
    
    stateMachine.Acknowledge()
    Assert.Equal(GameState.WaitingForInput, stateMachine.CurrentState)

    stateMachine.Input( ActionRequest.Move( Morgemil.Math.Vector2i.Identity) )
    testState()

