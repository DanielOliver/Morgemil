module Morgemil.Core.CharacterTagMatching

open Morgemil.Models

let isMatch (ancestry: Ancestry) (heritage: Heritage) : bool =
    (ancestry.Tags
     |> set
     |> Set.intersect (heritage.AncestryTags |> set)
     |> Set.count) = (heritage.AncestryTags.Count)
    && (heritage.Tags
        |> set
        |> Set.intersect (ancestry.HeritageTags |> set)
    |> Set.count) = (ancestry.HeritageTags.Count)
