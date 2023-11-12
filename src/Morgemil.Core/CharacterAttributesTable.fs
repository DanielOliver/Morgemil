namespace Morgemil.Core

open Morgemil.Models

type CharacterAttributesTable() =
    inherit Table<CharacterAttributes, CharacterID>(CharacterID, (fun (key) -> key.Key))

    member this.ByID(characterID: CharacterID) = characterID |> Table.GetRowByKey this
