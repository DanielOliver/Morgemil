module Morgemil.Core.Tests.CharacterTagMatchingTests

open Xunit
open Morgemil.Core
open Morgemil.Models
open Morgemil.Math

let ancestry1: Ancestry =
    { Ancestry.Adjective = "Adjective"
      Ancestry.Description = "Description"
      Ancestry.ID = AncestryID 1L
      Ancestry.Noun = "Noun"
      Ancestry.HeritageTags = set [ ]
      Ancestry.Tags = set [ CharacterTags.HasSkeleton  ] }

let ancestry2: Ancestry =
    { Ancestry.Adjective = "Adjective"
      Ancestry.Description = "Description"
      Ancestry.ID = AncestryID 2L
      Ancestry.Noun = "Noun"
      Ancestry.HeritageTags = set [ CharacterTags.Undead ]
      Ancestry.Tags = set [ CharacterTags.HasSkeleton  ] }

let heritage1: Heritage =
    { Heritage.ID = HeritageID 2L
      Adjective = "Adjective"
      Description = "Description"
      Noun = "Noun"
      Tags = set [ CharacterTags.Undead ]
      AncestryTags = set []
       }

let heritage2: Heritage =
    { Heritage.ID = HeritageID 3L
      Adjective = "Adjective"
      Description = "Description"
      Noun = "Noun"
      Tags = set [ CharacterTags.Undead ]
      AncestryTags = set [ CharacterTags.Undead  ]
       }

let heritage4: Heritage =
    { Heritage.ID = HeritageID 5L
      Adjective = "Adjective"
      Description = "Description"
      Noun = "Noun"
      Tags = set [ CharacterTags.Undead ]
      AncestryTags = set [ CharacterTags.HasSkeleton  ]
       }

[<Fact>]
let CharacterTagMatchingTests () =
    Assert.True (CharacterTagMatching.isMatch ancestry1 heritage1)
    Assert.False (CharacterTagMatching.isMatch ancestry1 heritage2)
    Assert.True (CharacterTagMatching.isMatch ancestry2 heritage4)

