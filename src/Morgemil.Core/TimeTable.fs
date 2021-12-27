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
    member this.Next = items.Min
    member this.NextAction = items.Min.NextAction

    member this.WaitingType: GameStateWaitingType =
        match items.Min.NextAction with
        | ActionArchetype.CharacterAfterInput
        | ActionArchetype.CharacterBeforeInput -> GameStateWaitingType.WaitingForEngine
        | ActionArchetype.CharacterEngineInput -> GameStateWaitingType.WaitingForAI
        | ActionArchetype.CharacterPlayerInput -> GameStateWaitingType.WaitingForInput

    interface IIndex<Character> with
        member this.AddRow next = next |> items.Add |> ignore

        member this.UpdateRow old next =
            old |> items.Remove |> ignore
            next |> items.Add |> ignore

        member this.Remove old = old |> items.Remove |> ignore
