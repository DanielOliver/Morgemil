module Morgemil.Core.MorTagMatching

open Morgemil.Models

let compatible
    (tags: Map<string, _>)
    (itemRequirements: Map<string, MorTagMatches>)
    (requirements: Map<string, MorTagMatches>)
    : bool =
    requirements
    |> Map.forall (fun key value ->
        match value, (tags |> Map.containsKey key) with
        | MorTagMatches.Has, true
        | MorTagMatches.Not, false -> true
        | MorTagMatches.Unique, _ -> key |> itemRequirements.ContainsKey |> not
        | _ -> false)

let mergeTags (priority: Map<string, MorTags>) (tags: Map<string, MorTags>) =
    let finalTags = System.Collections.Generic.Dictionary<string, MorTags>(priority)

    tags
    |> Map.iter (fun k v ->
        match finalTags.TryGetValue k with
        | true, value -> finalTags[k] <- (MorTags.merge value v)
        | false, _ -> finalTags.Add(k, v))

    finalTags |> Seq.map (fun t -> t.Key, t.Value) |> Map.ofSeq

let mergeMatchingTags (priority: Map<string, MorTagMatches>) (tags: Map<string, MorTagMatches>) =
    [| tags; priority |]
    |> Seq.concat
    |> Seq.map (fun kv -> kv.Key, kv.Value)
    |> Map.ofSeq

let isMatch (ancestry: Ancestry) (heritage: Heritage) : bool =
    (compatible ancestry.Tags ancestry.HeritageTags heritage.AncestryTags)
    && (compatible heritage.Tags heritage.AncestryTags ancestry.HeritageTags)
