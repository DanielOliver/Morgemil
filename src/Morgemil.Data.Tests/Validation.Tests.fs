module Morgemil.Data.Tests.Validation.Tests

open Morgemil.Data
open Xunit

[<Fact>]
let ``Try Validate Game With Validation`` () =
    let basePath = "./Game/"
    let rawDtoPhase0 = JsonReader.ReadGameFiles basePath
    Assert.Empty(rawDtoPhase0.Errors)
    Assert.True(rawDtoPhase0.Success)
    let rawDtoPhase1 = Validator.ValidateDtos rawDtoPhase0
    Assert.Empty(rawDtoPhase1.Errors)
    Assert.True(rawDtoPhase1.Success)

    let firstAncestry = rawDtoPhase1.Ancestries.Object[0].Object
    Assert.Equal(0L, firstAncestry.ID)
    Assert.Equal("Human", firstAncestry.Noun)
    Assert.Equal("Human", firstAncestry.Adjective)
    Assert.Equal("The kings of the overworld. How shall you fare in the darkness?", firstAncestry.Description)
