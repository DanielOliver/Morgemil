namespace Morgemil.Core

open Morgemil.Core
open Morgemil.Models
open Morgemil.Math

type LoopContext =
    { Characters: CharacterTable
      CharacterAttributes: CharacterAttributesTable
      TileMap: TileMap
      GameContext: GameContext TrackedEntity
      TimeTable: TimeTable }

type StaticLoopContext =
    { ScenarioData: ScenarioData
      Scenario: Scenario
      RNG: RNG.DefaultRNG }

module Loop =
    let processRequest
        (world: StaticLoopContext)
        (context: LoopContext)
        (builder: EventHistoryBuilder)
        (event: ActionRequest)
        : Step list * LoopContext =
        let mutable context = context

        builder {
            Tracked.Replace context.GameContext (fun t ->
                { t with
                    CurrentTimeTick = context.TimeTable.Next.NextTick })

            match event with
            | ActionRequest.Engine -> ()
            | ActionRequest.Pause characterID ->
                match characterID |> Table.TryGetRowByKey context.Characters with
                | None -> ()
                | Some pauseCharacter ->
                    Table.AddRow
                        context.Characters
                        { pauseCharacter with
                            NextTick = pauseCharacter.NextTick + 1000L<TimeTick>
                            NextAction = pauseCharacter.NextAction.NextInList pauseCharacter.TickActions }

                    yield { EventPause.CharacterID = characterID } |> ActionEvent.Pause

            | ActionRequest.Move actionRequestMove ->
                match actionRequestMove.CharacterID |> Table.TryGetRowByKey context.Characters with
                | None -> ()
                | Some moveCharacter ->
                    let oldPosition = moveCharacter.Position

                    let newPosition = oldPosition + actionRequestMove.Direction

                    let blocksMovement = context.TileMap.[newPosition] |> TileMap.blocksMovement

                    if blocksMovement then
                        yield
                            { CharacterID = moveCharacter.ID
                              OldPosition = oldPosition
                              RequestedPosition = newPosition }
                            |> ActionEvent.RefusedMove
                    else
                        let isFreeFromOtherCharacters =
                            context.Characters.ByPositions
                            |> Seq.where (fun (pos, c) -> pos = newPosition)
                            |> Seq.isEmpty

                        if not isFreeFromOtherCharacters then
                            yield
                                { CharacterID = moveCharacter.ID
                                  OldPosition = oldPosition
                                  RequestedPosition = newPosition }
                                |> ActionEvent.RefusedMove

                        else
                            Table.AddRow
                                context.Characters
                                { moveCharacter with
                                    Position = newPosition
                                    NextTick = moveCharacter.NextTick + 1000L<TimeTick>
                                    NextAction = moveCharacter.NextAction.NextInList moveCharacter.TickActions }

                            yield
                                { CharacterID = moveCharacter.ID
                                  OldPosition = oldPosition
                                  NewPosition = newPosition }
                                |> ActionEvent.AfterMove
            | ActionRequest.GoToNextLevel(characterID) ->
                match characterID |> Table.TryGetRowByKey context.Characters with
                | None -> ()
                | Some moveCharacter ->
                    if context.TileMap.[moveCharacter.Position] |> TileMap.isExitPoint then

                        let rng = world.RNG
                        let nextFloor = context.GameContext.Value.FloorID.TempNext

                        let (newTileMap, mapGenerationResults) =
                            FloorGenerator.Create
                                (world.ScenarioData.FloorGenerationParameters.Items |> Seq.head)
                                (world.ScenarioData.TileFeatures)
                                rng

                        Tracked.Replace context.TileMap (fun t -> newTileMap.TileMapData)
                        Tracked.Replace context.GameContext (fun t -> { t with FloorID = nextFloor })

                        Table.AddRow
                            context.Characters
                            { moveCharacter with
                                NextTick = moveCharacter.NextTick + 1000L<TimeTick>
                                NextAction = moveCharacter.NextAction.NextInList moveCharacter.TickActions
                                FloorID = nextFloor }

                        context.Characters
                        |> Table.Items
                        |> Seq.map (fun t ->
                            { t with
                                Position = (context.TileMap.EntryPoints |> Seq.head) + int (t.ID.Key)
                                FloorID = nextFloor })
                        |> Seq.iter (Table.AddRow context.Characters)

                        yield ActionEvent.MapChange

            yield ActionEvent.EndResponse 0
        },
        context

type Loop(world: StaticLoopContext, initialContext: LoopContext) =
    let mutable context: LoopContext = initialContext

    member this.WaitingType: GameStateWaitingType = context.TimeTable.WaitingType

    member this.NextMove =
        let direction =
            (RNG.RandomVector world.RNG (Point.create (2, 2))) - Point.create (1, 1)

        if direction = Point.Zero then
            ActionRequest.Pause context.TimeTable.Next.ID
        else
            ActionRequest.Move
                { ActionRequestMove.CharacterID = context.TimeTable.Next.ID
                  Direction = direction }

    member this.ProcessRequest(event: ActionRequest) : Step list =
        use builder =
            new EventHistoryBuilder(
                [ context.Characters
                  context.CharacterAttributes
                  context.GameContext
                  context.TileMap ]
            )

        let nextAction = context.TimeTable.NextAction

        match nextAction with
        | ActionArchetype.CharacterAfterInput
        | ActionArchetype.CharacterBeforeInput ->
            builder {
                let nextCharacter = context.TimeTable.Next

                Table.AddRow
                    context.Characters
                    { nextCharacter with
                        NextAction = nextCharacter.NextAction.NextInList nextCharacter.TickActions }

                yield ActionEvent.ActionArchetype nextAction
            }

        | ActionArchetype.CharacterEngineInput
        | ActionArchetype.CharacterPlayerInput ->
            let (steps, nextContext) = Loop.processRequest world context builder event
            context <- nextContext
            steps
