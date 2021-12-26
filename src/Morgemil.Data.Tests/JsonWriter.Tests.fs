module Morgemil.Data.Tests.JsonWriter.Tests

open Morgemil.Models
open Newtonsoft.Json
open Xunit
open Morgemil.Data.Convertors
open Morgemil.Data.ContractResolver

let settings = new JsonSerializerSettings()
settings.Converters.Add(new EnumUnionConvertor())
settings.Converters.Add(new SingleCaseUnionConverter())
settings.Converters.Add(new MultipleCaseUnionConverter())
settings.Converters.Add(new OptionConverter())
settings.Formatting <- Formatting.None
settings.ContractResolver <- new RowContractResolver()

[<Fact>]
let ``Try serialize and serialize odd case`` () =
    let item1 =
        { Item.ID = ItemID 50L
          Item.IsUnique = false
          Item.Noun = "one"
          Item.SubItem =
              SubItem.Weapon
                  { Weapon.BaseRange = 5<TileDistance>
                    Weapon.HandCount = 2<HandSlot>
                    Weapon.RangeType = WeaponRangeType.Melee
                    Weapon.Weight = 5M<Weight> } }

    let text1 =
        JsonConvert.SerializeObject(item1, settings)

    let expectedText =
        "{\"ID\":50,\"SubItem\":{\"Weapon\":[{\"RangeType\":\"Melee\",\"BaseRange\":5,\"HandCount\":2,\"Weight\":5.0}]},\"Noun\":\"one\",\"IsUnique\":false}"

    Assert.Equal(expectedText, text1)

    let deserializedItem =
        JsonConvert.DeserializeObject<Item>(expectedText, settings)

    Assert.Equal(item1, deserializedItem)


type TileFeature2 =
    { ID: TileFeatureID
      [<RecordIdAttribute>]
      Tile23: Tile
      [<RecordIdAttribute>]
      Tile234: Tile option }


    interface Relational.IRow with
        [<JsonIgnore>]
        member this.Key = this.ID.Key

//[<Fact>]
let ``Try serialize and serialize row case`` () =
    let tileFeature1 =
        { TileFeature2.ID = TileFeatureID 52L
          TileFeature2.Tile23 =
              { Tile.Description = "Desc2"
                Tile.Name = "Name2"
                Tile.Representation =
                    { TileRepresentation.AnsiCharacter = '1'
                      TileRepresentation.ForegroundColor = None
                      TileRepresentation.BackGroundColor = None }
                Tile.BlocksMovement = false
                Tile.BlocksSight = false
                Tile.ID = TileID 42L
                Tile.TileType = TileType.Ground }
          TileFeature2.Tile234 =
              { Tile.Description = "Desc2"
                Tile.Name = "Name2"
                Tile.Representation =
                    { TileRepresentation.AnsiCharacter = '1'
                      TileRepresentation.ForegroundColor = None
                      TileRepresentation.BackGroundColor = None }
                Tile.BlocksMovement = false
                Tile.BlocksSight = false
                Tile.ID = TileID 43L
                Tile.TileType = TileType.Ground }
              |> Some }

    let text1 =
        JsonConvert.SerializeObject(tileFeature1, settings)

    let expectedText =
        "{\"ID\":50,\"SubItem\":{\"Weapon\":[{\"RangeType\":\"Melee\",\"BaseRange\":5,\"HandCount\":2,\"Weight\":5.0}]},\"Noun\":\"one\",\"IsUnique\":false}"

    Assert.Equal(expectedText, text1)
