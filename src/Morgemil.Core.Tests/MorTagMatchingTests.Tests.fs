module Morgemil.Core.Tests.MorTagMatchingTests

open Xunit
open Morgemil.Core
open Morgemil.Models

let ancestry1: Ancestry =
    { Ancestry.Adjective = "Adjective"
      Ancestry.Description = "Description"
      Ancestry.ID = AncestryID 1L
      Ancestry.Noun = "Noun"
      Ancestry.RequireTags = Map.empty
      Ancestry.Tags =
        [ nameof (MorTags.NoSkeleton), MorTags.Custom
          nameof (MorTags.Modifier), MorTags.Modifier 6
          nameof (MorTags.Humanoid), (MorTags.Placeholder "Green") ]
        |> Map.ofList }

let ancestry2: Ancestry =
    { Ancestry.Adjective = "Adjective"
      Ancestry.Description = "Description"
      Ancestry.ID = AncestryID 2L
      Ancestry.Noun = "Noun"
      Ancestry.RequireTags = [ nameof MorTags.Undead, MorTagMatches.Has ] |> Map.ofList
      Ancestry.Tags = [ nameof MorTags.NoSkeleton, MorTags.Custom ] |> Map.ofList }

let heritage1: Heritage =
    { Heritage.ID = HeritageID 2L
      Adjective = "Adjective"
      Description = "Description"
      Noun = "Noun"
      Tags =
        [ nameof MorTags.Undead, MorTags.Custom
          nameof MorTags.Modifier, MorTags.Modifier 11
          nameof MorTags.Humanoid, (MorTags.Placeholder "Blue") ]
        |> Map.ofList
      RequireTags = Map.empty }

let heritage2: Heritage =
    { Heritage.ID = HeritageID 3L
      Adjective = "Adjective"
      Description = "Description"
      Noun = "Noun"
      Tags = [ nameof MorTags.Undead, MorTags.Custom ] |> Map.ofList
      RequireTags = [ nameof MorTags.Undead, MorTagMatches.Has ] |> Map.ofList }

let heritage4: Heritage =
    { Heritage.ID = HeritageID 5L
      Adjective = "Adjective"
      Description = "Description"
      Noun = "Noun"
      Tags =
        [ nameof MorTags.Undead, MorTags.Custom
          nameof MorTags.Humanoid, (MorTags.Placeholder "Red") ]
        |> Map.ofList
      RequireTags = [ nameof MorTags.NoSkeleton, MorTagMatches.Has ] |> Map.ofList }

[<Fact>]
let CharacterTagMatchingTests () =
    Assert.True(MorTagMatching.isMatch ancestry1 heritage1)
    Assert.False(MorTagMatching.isMatch ancestry1 heritage2)
    Assert.True(MorTagMatching.isMatch ancestry2 heritage4)

[<Fact>]
let CharacterTagMergingTests () =
    let mergedTags = MorTagMatching.mergeTags heritage1.Tags ancestry1.Tags

    match mergedTags[nameof (MorTags.Humanoid)] with
    | MorTags.Placeholder any -> Assert.Equal("Blue", any)
    | _ -> Assert.Fail "Wrong placeholder"

    match mergedTags[nameof (MorTags.Modifier)] with
    | MorTags.Modifier stat -> Assert.Equal(17, stat)
    | _ -> Assert.Fail "Wrong placeholder"

    Assert.True(mergedTags.ContainsKey(nameof (MorTags.NoSkeleton)))
    let mergedTags = MorTagMatching.mergeTags heritage4.Tags mergedTags

    match mergedTags[nameof (MorTags.Humanoid)] with
    | MorTags.Placeholder any -> Assert.Equal("Red", any)
    | _ -> Assert.Fail "Wrong placeholder"
