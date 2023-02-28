module Morgemil.Core.MorTagMatching

open Morgemil.Models

let compatible (tags: Map<string, string>) (requirements: Map<string, bool>) : bool =
    requirements
    |> Map.forall (fun key value ->
        match value, (tags |> Map.containsKey key) with
        | true, true
        | false, false -> true
        | true, false
        | false, true -> false)

let mergeTags (priority: Map<string, string>) (tags: Map<string, string>) =
    [| tags; priority |]
    |> Seq.concat
    |> Seq.map (fun kv -> kv.Key, kv.Value)
    |> Map.ofSeq

let isMatch (ancestry: Ancestry) (heritage: Heritage) : bool =
    (compatible ancestry.Tags heritage.AncestryTags)
    && (compatible heritage.Tags ancestry.HeritageTags)
