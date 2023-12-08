namespace Morgemil.Core

open System.Collections.Generic
open Morgemil.Core
open Morgemil.Models
open Morgemil.Models.Relational

type TimeComparer() =
    interface IComparer<Character> with
        member x.Compare(a, b) =
            let compareTime = int64(a.NextTick).CompareTo(b.NextTick)

            if compareTime = 0 then
                a.ID.Key.CompareTo(b.ID.Key)
            else
                compareTime

type TimeTable() =
    let items = SortedSet<Character>([], TimeComparer())
    /// This dictionary is a nasty way to keep track of what characters are currently working through their action ticks. Primarily because any step could mess with time.
    let mutable inProgress = Dictionary<CharacterID, Character>()

    member this.Next =
        if inProgress.Count = 0 then
            items.Min
        else
            (inProgress |> Seq.head).Value

    member this.NextAction = this.Next.NextAction

    member this.Items = items

    member this.WaitingType: GameStateWaitingType =
        match items.Min.NextAction with
        | ActionArchetype.CharacterAfterInput
        | ActionArchetype.CharacterBeforeInput -> GameStateWaitingType.WaitingForEngine
        | ActionArchetype.CharacterEngineInput -> GameStateWaitingType.WaitingForAI
        | ActionArchetype.CharacterPlayerInput -> GameStateWaitingType.WaitingForInput

    interface IIndex<Character> with
        member this.Add next = next |> items.Add |> ignore

        member this.Update old next =
            if next.NextAction <> next.TickActions.Head then
                inProgress[next.ID] <- next
            else
                inProgress.Remove(next.ID) |> ignore

            old |> items.Remove |> ignore
            next |> items.Add |> ignore

        member this.Remove old = old |> items.Remove |> ignore
