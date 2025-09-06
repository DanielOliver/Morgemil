namespace Morgemil.Core

open System.Collections.Generic
open Morgemil.Core
open Morgemil.Models
open Morgemil.Models.Relational

type TimeComparer() =
    interface IComparer<Entity> with
        member x.Compare(a, b) =
            match a.FloorActor, b.FloorActor with
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
        match entity.FloorActor with
        | ValueNone -> failwith "oof"
        | ValueSome floorActor -> floorActor.NextAction

    member this.NextFloorActor = this.Next

    member this.Next =
        if inProgress.Count = 0 then
            items.Min
        else
            (inProgress |> Seq.head).Value

    member this.NextAction = this.Next.FloorActor |> ValueOption.get |> (_.NextAction)

    member this.Items = items

    member this.WaitingType: GameStateWaitingType =
        match this.NextAction with
        | ActionArchetype.CharacterAfterInput
        | ActionArchetype.CharacterBeforeInput -> GameStateWaitingType.WaitingForEngine
        | ActionArchetype.CharacterEngineInput -> GameStateWaitingType.WaitingForAI
        | ActionArchetype.CharacterPlayerInput -> GameStateWaitingType.WaitingForInput items.Min.ID

    interface IIndex<Entity> with
        member this.Add next =
            match next.FloorActor with
            | ValueNone -> ()
            | ValueSome _ -> next |> items.Add |> ignore

        member this.Update old next =
            match old.FloorActor, next.FloorActor with
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
            match old.FloorActor with
            | ValueNone -> ()
            | ValueSome _ -> old |> items.Remove |> ignore
