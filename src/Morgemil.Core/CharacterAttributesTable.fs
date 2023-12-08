namespace Morgemil.Core

open Morgemil.Models

type CharacterAttributesTable() =
    inherit Table<CharacterAttributes, CharacterID>(CharacterID, (_.Key), StepItem.CharacterAttributes)

    member this.ByID(characterID: CharacterID) = characterID |> Table.GetRowByKey this
