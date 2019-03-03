namespace Morgemil.Models

open Morgemil.Models.Relational

type ScenarioData =
  { Races: IReadonlyTable<Race, RaceID>
    Tiles: IReadonlyTable<Tile, TileID>
//    Items: IReadonlyTable<Item, ItemID>
    RaceModifiers: IReadonlyTable<RaceModifier, RaceModifierID>
//    MonsterGenerationParameters: IReadonlyTable<MonsterGenerationParameter, MonsterGenerationParameterID>
//    FloorGenerationParameters: IReadonlyTable<FloorGenerationParameter, FloorGenerationParameterID>
//    Scenario: Scenario
  }
