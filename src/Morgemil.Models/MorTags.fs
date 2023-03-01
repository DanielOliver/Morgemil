namespace Morgemil.Models



[<RequireQualifiedAccess>]
type MorTags =
    | Custom
    | Placeholder of Any: string
    | Undead
    | HasSkeleton
    | Humanoid

module MorTags =
    let fromList data : Map<string, _> =
        data |> Seq.map (fun (k, v) -> (k.ToString(), v)) |> Map.ofSeq
