module Morgemil.Core.Tests.GameStateMachine

open Xunit
open Morgemil.Core
open Morgemil.Models
open Morgemil.Models.Relational


[<Fact>]
let ``Can transition states``() =
    let exampleLoop(request: ActionRequest): Step list =
        Assert.Equal(ActionRequest.Move(CharacterID 0L, Morgemil.Math.Vector2i.Identity), request)
        List.empty

    let stateMachine: IGameStateMachine = SimpleGameStateMachine exampleLoop :> IGameStateMachine
    Assert.Equal(GameStateType.WaitingForInput, stateMachine.CurrentState.GameStateType)

    let testState() = 
        while stateMachine.CurrentState.GameStateType = GameStateType.Processing do
            System.Threading.Thread.Sleep 500
        Assert.Equal(GameStateType.Results, stateMachine.CurrentState.GameStateType)
    
        match stateMachine.CurrentState with
        | GameState.Results (results, acknowledgeCallback) ->
            acknowledgeCallback()
        | _ -> ()
    
        while stateMachine.CurrentState.GameStateType <> GameStateType.WaitingForInput do
            System.Threading.Thread.Sleep 500
        Assert.Equal(GameStateType.WaitingForInput, stateMachine.CurrentState.GameStateType)    
    
    
    
    match stateMachine.CurrentState with
    | GameState.WaitingForInput (inputCallback) ->
        inputCallback ( ActionRequest.Move(CharacterID 0L, Morgemil.Math.Vector2i.Identity) )
    | _ -> ()
    testState()
    
    
    match stateMachine.CurrentState with
    | GameState.WaitingForInput (inputCallback) ->
        inputCallback ( ActionRequest.Move(CharacterID 0L, Morgemil.Math.Vector2i.Identity) )
    | _ -> ()
    
    testState()

