module Morgemil.Core.Tests.EventHistoryBuilder

open Morgemil.Models.Tracked
open Xunit
open Morgemil.Core
open Morgemil.Models
open Morgemil.Math
open Morgemil.Models.Relational

let exampleAncestry1 =
    { Ancestry.Noun = ""
      Ancestry.Adjective = ""
      Ancestry.Description = ""
      Ancestry.ID = AncestryID 50L
      Ancestry.Tags = Map.empty
      Ancestry.RequireTags = Map.empty }

let exampleItem1 =
    { Character.PlayerID = None
      Character.Ancestry = exampleAncestry1
      Character.Position = Vector2i.Identity
      Character.ID = CharacterID 51L
      Character.NextAction = Character.DefaultTickActions.Head
      Character.TickActions = Character.DefaultTickActions
      Character.Heritage = []
      Character.Floor = 1L<Floor>
      Character.NextTick = 1L<TimeTick>
      Character.Tags = Map.empty }

let exampleItem2 =
    { Character.PlayerID = None
      Character.Ancestry = exampleAncestry1
      Character.Position = Vector2i.Identity
      Character.ID = CharacterID 52L
      Character.NextAction = Character.DefaultTickActions.Head
      Character.TickActions = Character.DefaultTickActions
      Character.Heritage = []
      Character.Floor = 1L<Floor>
      Character.NextTick = 1L<TimeTick>
      Character.Tags = Map.empty }

let exampleItem3 =
    { Character.PlayerID = None
      Character.Ancestry = exampleAncestry1
      Character.Position = Vector2i.Identity
      Character.ID = CharacterID 53L
      Character.NextAction = Character.DefaultTickActions.Head
      Character.TickActions = Character.DefaultTickActions
      Character.Heritage = []
      Character.Floor = 1L<Floor>
      Character.NextTick = 1L<TimeTick>
      Character.Tags = Map.empty }

let exampleGameContext =
    { GameContext.CurrentTimeTick = 1L<TimeTick>
      Floor = 1L<Floor> }

[<Fact>]
let ``Can yield Results without updates`` () =
    let timeTable = TimeTable()
    let table1 = CharacterTable(timeTable)
    let trackedGameContext = TrackedEntity(exampleGameContext)

    use eventBuilder = new EventHistoryBuilder(table1, trackedGameContext)

    let results =
        eventBuilder {
            yield ActionEvent.Empty 1

            yield
                eventBuilder {
                    Tracked.Replace trackedGameContext (fun t -> { t with CurrentTimeTick = 2L<TimeTick> })

                    yield ActionEvent.Empty 2
                    Table.AddRow table1 exampleItem2
                    yield ActionEvent.Empty 3
                }

            Table.AddRow table1 exampleItem1
            yield ActionEvent.Empty 4
        }

    Assert.Equal(3, results.Length)

    Assert.Equal<Step list>(
        [ { Step.Event = ActionEvent.Empty 2
            Step.Updates =
              [ { TrackedEvent.OldValue =
                    { GameContext.CurrentTimeTick = 1L<TimeTick>
                      Floor = 1L<Floor> }
                  NewValue =
                    { GameContext.CurrentTimeTick = 2L<TimeTick>
                      Floor = 1L<Floor> } }
                |> StepItem.GameContext ] }
          { Step.Event = ActionEvent.Empty 3
            Step.Updates = [ exampleItem2 |> TableEvent.Added |> StepItem.Character ] }
          { Step.Event = ActionEvent.Empty 4
            Step.Updates = [ exampleItem1 |> TableEvent.Added |> StepItem.Character ] } ]
        |> List.rev,
        results
    )
