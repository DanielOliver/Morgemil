namespace Morgemil.Core

open System.Collections.Generic
open Morgemil.Core
open Morgemil.Models
open Morgemil.Models.Relational

type TimeComparer() =
    interface IComparer<Entity> with
        member x.Compare(a, b) =
            match Entity.floorActor a, Entity.floorActor b with
            | ValueNone, ValueNone
            | ValueNone, ValueSome _
            | ValueSome _, ValueNone -> 0
            | ValueSome aFloorActor, ValueSome bFloorActor ->
                let compareTime = int64(aFloorActor.NextTick).CompareTo(bFloorActor.NextTick)

                if compareTime = 0 then
                    a.ID.Key.CompareTo(b.ID.Key)
                else
                    compareTime

type TimeTable() =
    let items = SortedSet<Entity>([], TimeComparer())
    /// This dictionary is a nasty way to keep track of what characters are currently working through their action ticks. Primarily because any step could mess with time.
    let mutable inProgress = Dictionary<EntityID, Entity>()

    let nextAction (entity: Entity) =
        match Entity.floorActor entity with
        | ValueNone -> failwith "oof"
        | ValueSome floorActor -> floorActor.NextAction

    member this.NextFloorActor = this.Next |> Entity.floorActor |> ValueOption.get

    member this.Next =
        if inProgress.Count = 0 then
            items.Min
        else
            (inProgress |> Seq.head).Value

    member this.NextAction =
        this.Next |> Entity.floorActor |> ValueOption.get |> (_.NextAction)

    member this.Items = items

    member this.WaitingType: GameStateWaitingType =
        match this.NextAction with
        | ActionArchetype.CharacterAfterInput
        | ActionArchetype.CharacterBeforeInput -> GameStateWaitingType.WaitingForEngine
        | ActionArchetype.CharacterEngineInput -> GameStateWaitingType.WaitingForAI
        | ActionArchetype.CharacterPlayerInput -> GameStateWaitingType.WaitingForInput items.Min.ID

    interface IIndex<Entity> with
        member this.Add next =
            match Entity.floorActor next with
            | ValueNone -> ()
            | ValueSome _ -> next |> items.Add |> ignore

        member this.Update old next =
            match Entity.floorActor old, Entity.floorActor next with
            | ValueNone, ValueNone -> ()
            | ValueNone, ValueSome _ -> next |> items.Add |> ignore
            | ValueSome _, ValueNone -> old |> items.Remove |> ignore
            | ValueSome oldFloorActor, ValueSome newFloorActor ->
                if oldFloorActor <> newFloorActor then
                    if newFloorActor.NextAction <> newFloorActor.TickActions.Head then
                        inProgress[next.ID] <- next
                    else
                        inProgress.Remove(next.ID) |> ignore

                    old |> items.Remove |> ignore
                    next |> items.Add |> ignore

        member this.Remove old =
            match Entity.floorActor old with
            | ValueNone -> ()
            | ValueSome _ -> old |> items.Remove |> ignore
