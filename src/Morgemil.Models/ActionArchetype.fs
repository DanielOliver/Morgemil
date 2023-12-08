namespace Morgemil.Models

/// An Action Archetype fulfills AI and Player input requests, engine ticks, and is effectively as close to an events API as I have.
[<RequireQualifiedAccess>]
type ActionArchetype =
    | CharacterPlayerInput
    | CharacterEngineInput
    | CharacterBeforeInput
    | CharacterAfterInput

    /// Given a list of all actions, will return the next sequential choice.
    member this.NextInList(items: ActionArchetype list) =
        let index = 1 + (items |> List.findIndex (fun t -> t = this))

        if items.Length > index then
            items[index]
        else
            items[index - items.Length]
