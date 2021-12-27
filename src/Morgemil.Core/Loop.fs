namespace Morgemil.Core

open Morgemil.Core
open Morgemil.Models
open Morgemil.Math

type LoopContext =
    { Characters: CharacterTable
      TileMap: TileMap
      GameContext: GameContext TrackedEntity
      TimeTable: TimeTable }

type StaticLoopContext =
    { ScenarioData: ScenarioData
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
            Tracked.Replace
                context.GameContext
                (fun t ->
                    { t with
                          CurrentTimeTick = context.TimeTable.Next.NextTick })

            match event with
            | ActionRequest.Engine -> ()
            | ActionRequest.Pause characterID ->
                match characterID
                      |> Table.TryGetRowByKey context.Characters with
                | None -> ()
                | Some pauseCharacter ->
                    Table.AddRow
                        context.Characters
                        { pauseCharacter with
                              NextTick = pauseCharacter.NextTick + 1000L<TimeTick>
                              NextAction = pauseCharacter.NextAction.NextInList pauseCharacter.TickActions }

                    yield
                        { EventPause.CharacterID = characterID }
                        |> ActionEvent.Pause

            | ActionRequest.Move actionRequestMove ->
                match actionRequestMove.CharacterID
                      |> Table.TryGetRowByKey context.Characters with
                | None -> ()
                | Some moveCharacter ->
                    let oldPosition = moveCharacter.Position

                    let newPosition =
                        oldPosition + actionRequestMove.Direction

                    let blocksMovement =
                        context.TileMap.[newPosition]
                        |> TileMap.blocksMovement

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
            | ActionRequest.GoToNextLevel (characterID) ->
                match characterID
                      |> Table.TryGetRowByKey context.Characters with
                | None -> ()
                | Some moveCharacter ->
                    if context.TileMap.[moveCharacter.Position]
                       |> TileMap.isExitPoint then

                        let rng = world.RNG

                        let (newtileMap, mapGenerationResults) =
                            FloorGenerator.Create
                                (world.ScenarioData.FloorGenerationParameters.Items
                                 |> Seq.head)
                                (world.ScenarioData.TileFeatures)
                                rng

                        let createTileMapFromData (data: TileMapData) =
                            let result =
                                TileMap(
                                    Rectangle.create data.Size,
                                    data.DefaultTile,
                                    Array.zip data.Tiles data.TileFeatures
                                )

                            result

                        let tileMap =
                            createTileMapFromData newtileMap.TileMapData

                        context <- { context with TileMap = tileMap }

                        let items =
                            context.Characters |> Table.Items |> Seq.toArray

                        items
                        |> Seq.map
                            (fun t ->
                                { t with
                                      Position = context.TileMap.EntryPoints |> Seq.head })
                        |> Seq.iter (Table.AddRow context.Characters)

                        yield
                            { Characters =
                                  context.Characters
                                  |> Table.Items
                                  |> Seq.map
                                      (fun t ->
                                          { t with
                                                Position =
                                                    mapGenerationResults.EntranceCoordinate
                                                    + int (t.ID.Key)
                                                NextTick = t.NextTick + 1L<TimeTick> })
                                  |> Array.ofSeq
                              TileMapData = context.TileMap.TileMapData }
                            |> ActionEvent.MapChange

            yield ActionEvent.EndResponse 0
        },
        context

type Loop(world: StaticLoopContext, initialContext: LoopContext) =
    let mutable context: LoopContext = initialContext

    member this.WaitingType: GameStateWaitingType = context.TimeTable.WaitingType

    member this.NextMove =
        let direction =
            (RNG.RandomVector world.RNG (Vector2i.create (2, 2)))
            - Vector2i.create (1, 1)
        //        ActionRequest.Pause context.TimeTable.Next.ID
        if direction = Vector2i.Zero then
            ActionRequest.Pause context.TimeTable.Next.ID
        else
            ActionRequest.Move
                { ActionRequestMove.CharacterID = context.TimeTable.Next.ID
                  Direction = direction }

    member this.ProcessRequest(event: ActionRequest) : Step list =
        use builder =
            new EventHistoryBuilder(context.Characters, context.GameContext)

        match context.TimeTable.NextAction with
        | ActionArchetype.CharacterAfterInput
        | ActionArchetype.CharacterBeforeInput ->
            builder {
                let nextCharacter = context.TimeTable.Next

                Table.AddRow
                    context.Characters
                    { nextCharacter with
                          NextAction = nextCharacter.NextAction.NextInList nextCharacter.TickActions }

                yield ActionEvent.EndResponse 0
            }

        | ActionArchetype.CharacterEngineInput
        | ActionArchetype.CharacterPlayerInput ->

            let (steps, nextContext) =
                Loop.processRequest world context builder event

            context <- nextContext
            steps
