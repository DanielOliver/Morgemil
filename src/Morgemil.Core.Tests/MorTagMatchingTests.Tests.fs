module Morgemil.Core.Tests.MorTagMatchingTests

open Xunit
open Morgemil.Core
open Morgemil.Models

let ancestry1: Ancestry =
    { Ancestry.Adjective = "Adjective"
      Ancestry.Description = "Description"
      Ancestry.ID = AncestryID 1L
      Ancestry.Noun = "Noun"
      Ancestry.HeritageTags = Map.empty
      Ancestry.Tags = [ MorTags.HasSkeleton, ""; MorTags.Humanoid, "Green" ] |> MorTags.fromList }

let ancestry2: Ancestry =
    { Ancestry.Adjective = "Adjective"
      Ancestry.Description = "Description"
      Ancestry.ID = AncestryID 2L
      Ancestry.Noun = "Noun"
      Ancestry.HeritageTags = [ MorTags.Undead, true ] |> MorTags.fromList
      Ancestry.Tags = [ MorTags.HasSkeleton, "" ] |> MorTags.fromList }

let heritage1: Heritage =
    { Heritage.ID = HeritageID 2L
      Adjective = "Adjective"
      Description = "Description"
      Noun = "Noun"
      Tags = [ MorTags.Undead, ""; MorTags.Humanoid, "Blue" ] |> MorTags.fromList
      AncestryTags = Map.empty }

let heritage2: Heritage =
    { Heritage.ID = HeritageID 3L
      Adjective = "Adjective"
      Description = "Description"
      Noun = "Noun"
      Tags = [ MorTags.Undead, "" ] |> MorTags.fromList
      AncestryTags = [ MorTags.Undead, true ] |> MorTags.fromList }

let heritage4: Heritage =
    { Heritage.ID = HeritageID 5L
      Adjective = "Adjective"
      Description = "Description"
      Noun = "Noun"
      Tags = [ MorTags.Undead, ""; MorTags.Humanoid, "Red" ] |> MorTags.fromList
      AncestryTags = [ MorTags.HasSkeleton, true ] |> MorTags.fromList }

[<Fact>]
let CharacterTagMatchingTests () =
    Assert.True(MorTagMatching.isMatch ancestry1 heritage1)
    Assert.False(MorTagMatching.isMatch ancestry1 heritage2)
    Assert.True(MorTagMatching.isMatch ancestry2 heritage4)

[<Fact>]
let CharacterTagMergingTests () =
    let mergedTags = MorTagMatching.mergeTags heritage1.Tags ancestry1.Tags
    Assert.Equal("Blue", mergedTags[MorTags.Humanoid.ToString()])
    Assert.True(mergedTags.ContainsKey(MorTags.HasSkeleton.ToString()))
    let mergedTags = MorTagMatching.mergeTags heritage4.Tags mergedTags
    Assert.Equal("Red", mergedTags[MorTags.Humanoid.ToString()])
