namespace Morgemil.Core

open System.Collections.Generic
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

    interface IIndex<Character> with
        member this.AddRow next = next |> items.Add |> ignore

        member this.UpdateRow old next =
            old |> items.Remove |> ignore
            next |> items.Add |> ignore

        member this.Remove old = old |> items.Remove |> ignore
