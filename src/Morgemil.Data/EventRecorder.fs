module Morgemil.Data.EventRecorder

open Morgemil.Core
open Morgemil.Models

let fromCurrentDirectory () : EventRecorder =
    let file = "./game.txt"
    let options = JsonSettings.options

    System.IO.File.WriteAllText(file, "")

    let serialize item =
        System.Text.Json.JsonSerializer.Serialize(item, options)

    let recordSteps (steps: Step list) =
        let lines =
            steps
            |> Seq.collect (fun step ->
                ("Event " + (serialize step.Event))
                :: (step.Updates |> List.map (fun t -> "Update " + (serialize t))))

        System.IO.File.AppendAllLines(file, lines)

    let recordActionRequest (actionRequest: ActionRequest) =
        System.IO.File.AppendAllLines(file, [ "Request " + serialize actionRequest ])

    { RecordSteps = recordSteps
      RecordActionRequest = recordActionRequest }
