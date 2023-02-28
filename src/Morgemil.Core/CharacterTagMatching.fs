module Morgemil.Core.CharacterTagMatching

open Morgemil.Models

let isMatch (ancestry: Ancestry) (heritage: Heritage) : bool =
    ancestry.HeritageTags
    |> Map.forall (fun key value ->
        match value, (heritage.Tags |> Map.containsKey key) with
        | true, true
        | false, false -> true
        | true, false
        | false, true -> false)
    && heritage.AncestryTags
       |> Map.forall (fun key value ->
           match value, (ancestry.Tags |> Map.containsKey key) with
           | true, true
           | false, false -> true
           | true, false
           | false, true -> false)
