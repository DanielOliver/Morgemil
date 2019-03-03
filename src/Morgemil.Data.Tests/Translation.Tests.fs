module Morgemil.Data.Tests.Translation.TestsTile

open Morgemil.Data
open Morgemil.Data.DTO
open Morgemil.Math
open Xunit

let isError = function | Result.Ok _ -> false | Result.Error _ -> true
let isOk = function | Result.Ok _ -> false | Result.Error _ -> true

[<Fact>]
let ``ColorToDto Tests`` () =
    let s1 = """{ "A": 255, "B": 253, "G": 252, "R": 249 } """
    let deserial1 = Newtonsoft.Json.JsonConvert.DeserializeObject<DTO.Color> s1            
    let expected1 = Color.From(249, 252, 253, 255) |> Translation.ColorToDto
    Assert.Equal(expected1, deserial1)
    
[<Fact>]
let ``ColorFromDto Tests`` () =
    let s1 = """{"A":255,"B":253,"G":252,"R":249}"""            
    let expected1 = Color.From(249, 252, 253, 255) |> Translation.ColorToDto
    let serial1 = Newtonsoft.Json.JsonConvert.SerializeObject expected1
    Assert.Equal(s1, serial1)


[<Fact>]
let ``Try Translate Game``() =
    let basePath = "./Game/"
    let rawDtoPhase0 = JsonReader.ReadGameFiles basePath
    Assert.Empty(rawDtoPhase0.Errors)
    Assert.True(rawDtoPhase0.Success)
    let rawDtoPhase2 =  Translation.TranslateFromDtosToPhase2 rawDtoPhase0
    Assert.NotEmpty(rawDtoPhase2.Races)
    Assert.NotEmpty(rawDtoPhase2.Tiles)
    Assert.NotEmpty(rawDtoPhase2.RaceModifiers)
