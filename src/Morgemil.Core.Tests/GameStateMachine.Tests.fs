module Morgemil.Core.Tests.GameStateMachine

open Xunit
open Morgemil.Core
open Morgemil.Models
open Morgemil.Models.Relational

[<Fact>]
let ``Can transition states``() =
    let exampleLoop(request: ActionRequest): Character Step list =
        Assert.Equal(ActionRequest.Move(CharacterID 0L, Morgemil.Math.Vector2i.Identity), request)
        List.empty

    let stateMachine: IGameStateMachine = SimpleGameStateMachine exampleLoop :> IGameStateMachine
    Assert.Equal(GameState.WaitingForInput, stateMachine.CurrentState)

    let testState() = 
        while stateMachine.CurrentState = GameState.Processing do
            System.Threading.Thread.Sleep 500
        Assert.Equal(GameState.Results List.empty, stateMachine.CurrentState)

    stateMachine.Input( ActionRequest.Move(CharacterID 0L, Morgemil.Math.Vector2i.Identity) )
    testState()
    
    stateMachine.Acknowledge()
    Assert.Equal(GameState.WaitingForInput, stateMachine.CurrentState)

    stateMachine.Input( ActionRequest.Move(CharacterID 0L, Morgemil.Math.Vector2i.Identity) )
    testState()

