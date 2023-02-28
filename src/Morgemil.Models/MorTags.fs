namespace Morgemil.Models


[<RequireQualifiedAccess>]
type MorTags =
    | Undead
    | HasSkeleton
    | Humanoid

module MorTags =
    let fromMap (data: Map<MorTags, _>) : Map<string, _> =
        data |> Seq.map (fun t -> (t.Key.ToString(), t.Value)) |> Map.ofSeq

    let fromList data : Map<string, _> =
        data |> Seq.map (fun (k, v) -> (k.ToString(), v)) |> Map.ofSeq
