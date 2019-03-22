namespace Morgemil.Core

open Morgemil.Models

type CharacterTable() =
    inherit Table<Character, CharacterID>(CharacterID, fun(key) -> key.Key)

    member this.ByPositions = this |> Table.Items |> Seq.map(fun t -> t.Position, t)

    member this.ByID(characterID: CharacterID) = characterID |> Table.GetRowByKey this
