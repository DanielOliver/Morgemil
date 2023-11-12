namespace Morgemil.Core

open Morgemil.Models

type EventRecorder =
    { RecordSteps: Step list -> unit
      RecordActionRequest: ActionRequest -> unit }
