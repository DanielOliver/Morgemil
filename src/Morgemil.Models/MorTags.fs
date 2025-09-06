namespace Morgemil.Models

[<RequireQualifiedAccess>]
type MorTagMatches =
    /// Checks whether the match has this tag. Doesn't care about values.
    | Has
    /// Checks whether the match lacks this tag. Doesn't care about values.
    | Not
    /// Checks whether the match has this RequireTag also. Expects this to be unique.
    | Unique

/// Conventions:
/// * Usage of these in data should be as sparse as possible. Assume use of these to be the rare case.
/// * If prefix "No" is used, then assume that's the more likely unusual case.
[<RequireQualifiedAccess>]
type MorTags =
    | Custom
    | Placeholder of Any: string
    | Playable
    | Undead
    | NoSkeleton
    | NoBlood
    | Humanoid
    | Modifier of Stat: int

module MorTags =
    let merge (priority: MorTags) (tag: MorTags) : MorTags =
        match priority, tag with
        | MorTags.Modifier statP, MorTags.Modifier stat -> MorTags.Modifier(stat + statP)
        | _, _ -> priority

    let mergeMatcher (key: string) (priority: MorTagMatches) (tag: MorTagMatches) : MorTagMatches =
        match priority, tag with
        | _, _ -> priority
