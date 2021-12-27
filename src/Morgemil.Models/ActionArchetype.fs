namespace Morgemil.Models

[<RequireQualifiedAccess>]
type ActionArchetype =
    | CharacterPlayerInput
    | CharacterEngineInput
    | CharacterBeforeInput
    | CharacterAfterInput

    member this.NextInList (items: ActionArchetype list) =
        let index = 1 + (items |> List.findIndex (fun t -> t = this))
        if items.Length > index then
            items.[index]
        else
            items.[index - items.Length]
