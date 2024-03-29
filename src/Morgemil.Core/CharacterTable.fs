namespace Morgemil.Core

open Morgemil.Models

type CharacterTable(timeTable: TimeTable) as this =
    inherit Table<Character, CharacterID>(CharacterID, (_.Key), StepItem.Character)

    do this.AddIndex timeTable

    member this.ByPositions = this |> Table.Items |> Seq.map (fun t -> t.Position, t)

    member this.ByID(characterID: CharacterID) = characterID |> Table.GetRowByKey this

    member this.ByPlayerID(playerID: PlayerID) =
        this
        |> Table.Items
        |> Seq.tryFind (fun t -> t.PlayerID.IsSome && t.PlayerID.Value = playerID)

    member this.ByTicks = this |> Table.Items |> Seq.sortBy (_.NextTick)
