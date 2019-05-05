module Morgemil.Core.Tests.EventHistoryBuilder

open Xunit
open Morgemil.Core
open Morgemil.Models
open Morgemil.Math
open Morgemil.Models.Relational

let exampleRace1 = {
        Race.Noun = ""
        Race.Adjective = ""
        Race.Description = ""
        Race.ID = RaceID 50L
    }

let exampleItem1 = {
    Character.PlayerID = None
    Character.Race = exampleRace1
    Character.Position = Vector2i.Identity
    Character.ID = CharacterID 51L
    Character.RaceModifier = None
}

let exampleItem2 = {
    Character.PlayerID = None
    Character.Race = exampleRace1
    Character.Position = Vector2i.Identity
    Character.ID = CharacterID 52L
    Character.RaceModifier = None
}

let exampleItem3 = {
    Character.PlayerID = None
    Character.Race = exampleRace1
    Character.Position = Vector2i.Identity
    Character.ID = CharacterID 53L
    Character.RaceModifier = None
}

[<Fact>]
let ``Can yield Results without updates``() =
    let table1 = CharacterTable()
    use eventBuilder = new EventHistoryBuilder(table1)
    let results =
        eventBuilder {
            yield ActionEvent.Empty 1
            yield eventBuilder {
                yield ActionEvent.Empty 2
                Table.AddRow table1 exampleItem2
                yield ActionEvent.Empty 3
            }
            Table.AddRow table1 exampleItem1
            yield ActionEvent.Empty 4
        }
    Assert.Equal(4, results.Length)
    Assert.Equal< Step list>(
        [
            {
                Step.Event = ActionEvent.Empty 1
                Step.Updates = []
            }
            {
                Step.Event = ActionEvent.Empty 2
                Step.Updates = []
            }
            {
                Step.Event = ActionEvent.Empty 3
                Step.Updates = [ exampleItem2 |> TableEvent.Added |> StepItem.Character ]
            }
            {
                Step.Event = ActionEvent.Empty 4
                Step.Updates = [ exampleItem1 |> TableEvent.Added |> StepItem.Character ]
            }
        ] |> List.rev,
        results)
