module Morgemil.Core.Tests.GameStateMachine

open Xunit
open Morgemil.Core
open Morgemil.Models


[<Fact>]
let ``Can transition states`` () =
    let exampleLoop (request: ActionRequest) : Step list =
        Assert.Equal(
            (ActionRequest.Move
                { ActionRequestMove.CharacterID = CharacterID 0L
                  ActionRequestMove.Direction = Morgemil.Math.Vector2i.Identity }),
            request
        )

        List.empty

    let scenarioData =
        { ScenarioData.Items = Table.CreateReadonlyTable(fun (ItemID id) -> id) []
          ScenarioData.Races = Table.CreateReadonlyTable(fun (RaceID id) -> id) []
          ScenarioData.Tiles = Table.CreateReadonlyTable(fun (TileID id) -> id) []
          ScenarioData.TileFeatures = Table.CreateReadonlyTable(fun (TileFeatureID id) -> id) []
          ScenarioData.RaceModifiers = Table.CreateReadonlyTable(fun (RaceModifierID id) -> id) []
          ScenarioData.FloorGenerationParameters =
              Table.CreateReadonlyTable(fun (FloorGenerationParameterID id) -> id) []
          ScenarioData.MonsterGenerationParameters =
              Table.CreateReadonlyTable(fun (MonsterGenerationParameterID id) -> id) []
          ScenarioData.Aspects = Table.CreateReadonlyTable(fun (AspectID id) -> id) [] }

    let stateMachine: IGameStateMachine =
        SimpleGameStateMachine(exampleLoop, (fun () -> GameStateWaitingType.WaitingForInput (CharacterID 0L)), Table.EmptyScenarioData)
        :> IGameStateMachine

    Assert.Equal(GameStateType.WaitingForInput, stateMachine.CurrentState.GameStateType)

    let testState () =
        while stateMachine.CurrentState.GameStateType
              <> GameStateType.WaitingForInput do
            System.Threading.Thread.Sleep 100

            match stateMachine.CurrentState with
            | GameState.Results (results, acknowledge) -> acknowledge ()
            | _ -> ()

        Assert.Equal(GameStateType.WaitingForInput, stateMachine.CurrentState.GameStateType)



    match stateMachine.CurrentState with
    | GameState.WaitingForInput (inputCallback) ->
        inputCallback (
            ActionRequest.Move
                { ActionRequestMove.CharacterID = CharacterID 0L
                  ActionRequestMove.Direction = Morgemil.Math.Vector2i.Identity }
        )
    | _ -> ()

    testState ()


    match stateMachine.CurrentState with
    | GameState.WaitingForInput (inputCallback) ->
        inputCallback (
            ActionRequest.Move
                { ActionRequestMove.CharacterID = CharacterID 0L
                  ActionRequestMove.Direction = Morgemil.Math.Vector2i.Identity }
        )
    | _ -> ()

    testState ()
