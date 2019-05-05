module Morgemil.Data.Tests.JsonWriter.Tests

open Morgemil.Models
open Newtonsoft.Json
open Xunit
open Morgemil.Data.Convertors

let settings = new JsonSerializerSettings();
settings.Converters.Add(new EnumUnionConvertor())
settings.Converters.Add(new SingleCaseUnionConverter())
settings.Converters.Add(new MultipleCaseUnionConverter())
settings.Converters.Add(new OptionConverter())
settings.Formatting <- Formatting.None;

[<Fact>]
let ``Try serialize and serialize odd case``() =
    let item1 = {
        Item.ID = ItemID 50L
        Item.IsUnique = false
        Item.Noun = "one"
        Item.SubItem = SubItem.Weapon {
            Weapon.BaseRange = 5<TileDistance>
            Weapon.HandCount = 2<HandSlot>
            Weapon.RangeType = WeaponRangeType.Melee
            Weapon.Weight = 5M<Weight>
        }
    }
    let text1 = JsonConvert.SerializeObject(item1, settings)
    let expectedText = "{\"ID\":50,\"SubItem\":{\"Weapon\":[{\"RangeType\":\"Melee\",\"BaseRange\":5,\"HandCount\":2,\"Weight\":5.0}]},\"Noun\":\"one\",\"IsUnique\":false,\"ItemType\":\"Weapon\"}"
    Assert.Equal(expectedText, text1)
