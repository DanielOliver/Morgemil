module Morgemil.Data.DataLoader

open Morgemil.Models

let LoadScenarioData (callback: ScenarioData -> unit) =
    async {
        let rawGameDataPhase0 =
            JsonReader.ReadGameFiles(
                if System.IO.Directory.Exists "../Morgemil.Data/Game" then
                    "../Morgemil.Data/Game"
                else
                    "./Game"
            )

        let scenarioData = Translation.FromDTO.TranslateFromDtosToScenario rawGameDataPhase0

        callback scenarioData
    }
    |> Async.Start
