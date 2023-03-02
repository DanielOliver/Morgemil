namespace Morgemil.Models


[<RequireQualifiedAccess>]
type MorTagMatches =
    | Has
    | Not
    | Unique

[<RequireQualifiedAccess>]
type MorTags =
    | Custom
    | Placeholder of Any: string
    | Undead
    | HasSkeleton
    | Humanoid
    | Modifier of Stat: int


module MorTags =
    // let private stringNames =
    //     FSharpType.GetUnionCases(typeof<MorTags>)
    //     |> Array.map (fun t -> t.Name, t)
    //     |> Map.ofArray

    let merge (priority: MorTags) (tag: MorTags) : MorTags =
        match priority, tag with
        | MorTags.Modifier statP, MorTags.Modifier stat -> MorTags.Modifier(stat + statP)
        | _, _ -> priority

    let mergeMatcher (key: string) (priority: MorTagMatches) (tag: MorTagMatches) : MorTagMatches =
        match priority, tag with
        | _, _ -> priority
