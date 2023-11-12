namespace Morgemil.Core

open Morgemil.Models

type EventRecorder =
    { RecordSteps: Step list -> unit
      RecordActionRequest: ActionRequest -> unit }

    static member Ignore: EventRecorder =
        { RecordSteps = ignore
          RecordActionRequest = ignore }
