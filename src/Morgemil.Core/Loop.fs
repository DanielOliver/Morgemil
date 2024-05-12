namespace Morgemil.Core

open Morgemil.Core
open Morgemil.Models
open Morgemil.Math
open Morgemil.Models.Relational

type LoopContext =
    { TileMap: TileMap
      GameContext: GameContext TrackedEntity
      Entities: EntityTable
      TimeTable: TimeTable }

    member this.ApplyStepItem(stepItem: StepItem) =
        match stepItem with
        | StepItem.GameContext context -> Tracked.Update this.GameContext context.NewValue
        | StepItem.CompleteMapChange context -> Tracked.Update this.TileMap context.NewValue
        | StepItem.TileInstance _ -> failwith "NotImplemented"
        | StepItem.Entity entity ->
            match entity with
            | TableEvent.Added(row) -> Table.AddRow this.Entities row
            | TableEvent.Updated(_, row) -> Table.AddRow this.Entities row
            | TableEvent.Removed(row) -> Table.RemoveRow this.Entities row
        | StepItem.EntityProperties entityProperties ->
            match entityProperties with
            | TableEvent.Added _ -> failwithf "Adding EntityProperties is unsupported"
            | TableEvent.Updated(_, row) -> this.Entities.Update row
            | TableEvent.Removed _ -> failwithf "Removing EntityProperties is unsupported"

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
                    CurrentTimeTick = context.TimeTable.NextFloorActor.NextTick })

            match event with
            | ActionRequest.Engine -> ()
            | ActionRequest.Pause entityID ->
                match entityID |> Table.TryGetRowByKey context.Entities with
                | None -> ()
                | Some pauseCharacter ->
                    match pauseCharacter |> Entity.floorActor with
                    | ValueNone -> ()
                    | ValueSome entityFloorActor ->
                        context.Entities.Update
                            { entityFloorActor with
                                NextTick = entityFloorActor.NextTick + 1000L<TimeTick>
                                NextAction = entityFloorActor.NextAction.NextInList entityFloorActor.TickActions }

                        yield { EventPause.EntityID = pauseCharacter.ID } |> ActionEvent.Pause

            | ActionRequest.Move actionRequestMove ->
                match actionRequestMove.EntityID |> Table.TryGetRowByKey context.Entities with
                | None -> ()
                | Some moveCharacter ->
                    let floorLocation = moveCharacter |> Entity.floorLocation |> ValueOption.get

                    let newPosition = floorLocation.Position + actionRequestMove.Direction

                    let blocksMovement = context.TileMap[newPosition] |> TileMap.blocksMovement

                    if blocksMovement then
                        yield
                            { EntityID = moveCharacter.ID
                              OldPosition = floorLocation.Position
                              RequestedPosition = newPosition }
                            |> ActionEvent.RefusedMove
                    else
                        let isFreeFromOtherCharacters =
                            context.Entities
                            |> Table.Items
                            |> Seq.where (fun e ->
                                (e |> Entity.floorLocation |> ValueOption.get).Position = newPosition)
                            |> Seq.isEmpty

                        if not isFreeFromOtherCharacters then
                            yield
                                { EntityID = moveCharacter.ID
                                  OldPosition = floorLocation.Position
                                  RequestedPosition = newPosition }
                                |> ActionEvent.RefusedMove

                        else

                            let floorActor = (moveCharacter |> Entity.floorActor |> ValueOption.get)

                            context.Entities.Update
                                { floorActor with
                                    NextTick = floorActor.NextTick + 1000L<TimeTick>
                                    NextAction = floorActor.NextAction.NextInList floorActor.TickActions }

                            context.Entities.Update
                                { floorLocation with
                                    Position = newPosition }

                            yield
                                { EntityID = moveCharacter.ID
                                  OldPosition = floorLocation.Position
                                  NewPosition = newPosition }
                                |> ActionEvent.AfterMove
            | ActionRequest.GoToNextLevel(entityID) ->
                match entityID |> Table.TryGetRowByKey context.Entities with
                | None -> ()
                | Some moveCharacter ->
                    if
                        context.TileMap[moveCharacter |> Entity.floorLocation |> ValueOption.get |> (_.Position)]
                        |> TileMap.isExitPoint
                    then

                        let rng = world.RNG
                        let nextFloor = context.GameContext.Value.FloorID.TempNext

                        let newTileMap, mapGenerationResults =
                            FloorGenerator.Create
                                (world.ScenarioData.FloorGenerationParameters.Items |> Seq.head)
                                world.ScenarioData.TileFeatures
                                rng

                        Tracked.Replace context.TileMap (fun t -> newTileMap.TileMapData)
                        Tracked.Replace context.GameContext (fun t -> { t with FloorID = nextFloor })


                        match moveCharacter |> Entity.floorActor, moveCharacter |> Entity.floorLocation with
                        | ValueNone, ValueNone
                        | ValueNone, ValueSome _
                        | ValueSome _, ValueNone -> ()
                        | ValueSome entityFloorActor, ValueSome entityFloorLocation ->
                            context.Entities.Update
                                { entityFloorActor with
                                    NextTick = entityFloorActor.NextTick + 1000L<TimeTick>
                                    NextAction = entityFloorActor.NextAction.NextInList entityFloorActor.TickActions }

                            context.Entities.Update
                                { entityFloorLocation with
                                    FloorID = nextFloor }

                        context.Entities
                        |> Table.Items
                        |> Seq.map (fun t ->
                            { EntityFloorLocation.ID = t.ID
                              Position = (context.TileMap.EntryPoints |> Seq.head) + int t.ID.Key
                              FloorID = nextFloor })
                        |> Seq.iter (context.Entities.Update)

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
                { ActionRequestMove.EntityID = context.TimeTable.Next.ID
                  Direction = direction }

    member this.ProcessRequest(event: ActionRequest) : Step list =
        use builder =
            new EventHistoryBuilder([ context.GameContext; context.TileMap; context.Entities ])

        let nextAction = context.TimeTable.NextAction

        match nextAction with
        | ActionArchetype.CharacterAfterInput
        | ActionArchetype.CharacterBeforeInput ->
            builder {
                let nextCharacter = context.TimeTable.NextFloorActor

                context.Entities.Update
                    { nextCharacter with
                        NextAction = nextCharacter.NextAction.NextInList nextCharacter.TickActions }

                yield ActionEvent.ActionArchetype nextAction
            }

        | ActionArchetype.CharacterEngineInput
        | ActionArchetype.CharacterPlayerInput ->
            let steps, nextContext = Loop.processRequest world context builder event
            context <- nextContext
            steps
