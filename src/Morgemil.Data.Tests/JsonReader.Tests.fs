module Morgemil.Data.Tests.JsonReader.Tests

open Morgemil.Data
open Newtonsoft.Json
open Xunit

[<Fact>]
let ``Try Read Game`` () =
    let basePath = "./Game/"
    let rawDtoList = JsonReader.ReadGameFiles basePath
    Assert.Empty(rawDtoList.Errors)
    Assert.True(rawDtoList.Success)

[<Fact>]
let ``Try serialize and deserialize JSON RNG`` () =
    let rng = Morgemil.Math.RNG.SeedRNG 500
    rng.Next() |> ignore
    let serialized = JsonConvert.SerializeObject(rng)
    let expectedNext = rng.Next()
    Assert.Equal(160506331, expectedNext)

    let rngDeserialized =
        JsonConvert.DeserializeObject<Morgemil.Math.RNG.DefaultRNG>(serialized)

    let actualNext = rngDeserialized.Next()
    Assert.Equal(expectedNext, actualNext)
