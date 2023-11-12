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
      Character.Position = Point.Identity
      Character.ID = CharacterID 51L
      Character.NextAction = Character.DefaultTickActions.Head
      Character.TickActions = Character.DefaultTickActions
      Character.Floor = 1L<Floor>
      Character.NextTick = 1L<TimeTick> }

let exampleItem2 =
    { Character.PlayerID = None
      Character.Position = Point.Identity
      Character.ID = CharacterID 52L
      Character.NextAction = Character.DefaultTickActions.Head
      Character.TickActions = Character.DefaultTickActions
      Character.Floor = 1L<Floor>
      Character.NextTick = 1L<TimeTick> }

let exampleItem3 =
    { Character.PlayerID = None
      Character.Position = Point.Identity
      Character.ID = CharacterID 53L
      Character.NextAction = Character.DefaultTickActions.Head
      Character.TickActions = Character.DefaultTickActions
      Character.Floor = 1L<Floor>
      Character.NextTick = 1L<TimeTick> }


let defaultTile: Tile =
    { ID = TileID 1L
      Name = "Dungeon Wall"
      TileType = TileType.Solid
      Description = "Dungeon floors are rock, paved cobblestone, and very slipper when bloody."
      BlocksMovement = true
      BlocksSight = true
      Representation =
        { AnsiCharacter = '#'
          ForegroundColor = Some <| Color.From(200, 200, 200, 255)
          BackGroundColor = Some <| Color.Black } }

let exampleGameContext =
    { GameContext.CurrentTimeTick = 1L<TimeTick>
      Floor = 1L<Floor> }

[<Fact>]
let ``Can yield Results without updates`` () =
    let timeTable = TimeTable()
    let table1 = CharacterTable(timeTable)
    let trackedGameContext = TrackedEntity(exampleGameContext)
    let tileMap = TileMap(Rectangle.create (10, 10), defaultTile)
    let attributesTable1 = CharacterAttributesTable()

    use eventBuilder =
        new EventHistoryBuilder(table1, trackedGameContext, tileMap, attributesTable1)

    let results =
        eventBuilder {
            yield ActionEvent.Empty 1

            yield
                eventBuilder {
                    Tracked.Replace trackedGameContext (fun t ->
                        { t with
                            CurrentTimeTick = 2L<TimeTick> })

                    Tracked.Replace trackedGameContext (fun t ->
                        { t with
                            CurrentTimeTick = 3L<TimeTick> })

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
                |> StepItem.GameContext
                { TrackedEvent.OldValue =
                    { GameContext.CurrentTimeTick = 2L<TimeTick>
                      Floor = 1L<Floor> }
                  NewValue =
                    { GameContext.CurrentTimeTick = 3L<TimeTick>
                      Floor = 1L<Floor> } }
                |> StepItem.GameContext ] }
          { Step.Event = ActionEvent.Empty 3
            Step.Updates = [ exampleItem2 |> TableEvent.Added |> StepItem.Character ] }
          { Step.Event = ActionEvent.Empty 4
            Step.Updates = [ exampleItem1 |> TableEvent.Added |> StepItem.Character ] } ]
        |> List.rev,
        results
    )
