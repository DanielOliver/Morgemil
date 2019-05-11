module Morgemil.Core.Tests.GameBuilderMachine

open Xunit
open Morgemil.Core
open Morgemil.Models

[<Fact>]
let ``Can transition states``() =
    let scenarioData = {
        ScenarioData.Items = Table.CreateReadonlyTable (fun (ItemID id) -> id) []
        ScenarioData.Races = Table.CreateReadonlyTable (fun (RaceID id) -> id) []
        ScenarioData.Tiles = Table.CreateReadonlyTable (fun (TileID id) -> id) []
        ScenarioData.TileFeatures = Table.CreateReadonlyTable (fun (TileFeatureID id) -> id) []
        ScenarioData.RaceModifiers = Table.CreateReadonlyTable (fun (RaceModifierID id) -> id) []
        ScenarioData.FloorGenerationParameters = Table.CreateReadonlyTable (fun (FloorGenerationParameterID id) -> id) []
        ScenarioData.MonsterGenerationParameters = Table.CreateReadonlyTable (fun (MonsterGenerationParameterID id) -> id) []
    }
    
    let loadScenarioData (callback: ScenarioData -> unit) =
        callback scenarioData
    
    let machine: IGameBuilder = SimpleGameBuilderMachine(loadScenarioData) :> IGameBuilder
    Assert.Equal(GameBuilderStateType.SelectScenario, machine.CurrentState.GameBuilderStateType)
    
    match machine.CurrentState with
    | GameBuilderState.SelectScenario (scenarioList, scenarioCallback) ->
        scenarioCallback (scenarioList.Head)
    | _ -> ()            

    while machine.CurrentState.GameBuilderStateType = GameBuilderStateType.LoadedScenarioData do
        System.Threading.Thread.Sleep 200
    Assert.Equal(GameBuilderStateType.WaitingForCurrentPlayer, machine.CurrentState.GameBuilderStateType)
    
    match machine.CurrentState with
    | GameBuilderState.WaitingForCurrentPlayer (addPlayer) ->
        addPlayer (RaceID 1L)
    | _ -> ()
    
    Assert.Equal(GameBuilderStateType.LoadingGameProgress, machine.CurrentState.GameBuilderStateType)
    
    ()
    
    