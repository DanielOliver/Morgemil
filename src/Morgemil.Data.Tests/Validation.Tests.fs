module Morgemil.Data.Tests.Validation.Tests


open Morgemil.Data
open Xunit

[<Fact>]
let ``Try Validate Game``() =
    let basePath = "./Game/"
    let rawDtoPhase0 = JsonReader.ReadGameFiles basePath
    Assert.Empty(rawDtoPhase0.Errors)
    Assert.True(rawDtoPhase0.Success)
    let rawDtoPhase1 =  Validator.ValidateDtos rawDtoPhase0
    Assert.Empty(rawDtoPhase1.Errors)
    Assert.True(rawDtoPhase1.Success)
