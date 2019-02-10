namespace Morgemil.Models

type ScenarioData =
  { Races: Map<RaceID,Race>
    Tiles: Map<TileID,Tile>
    Items: Map<ItemID,Item>
    RaceModifiers: Map<RaceModifierID,RaceModifier>
    RaceModifierLinks: Map<RaceModifierLinkID,RaceModifierLink>
    FloorGenerationParameters: Map<FloorGenerationParameterID,FloorGenerationParameter>
    Scenario: Scenario
  }
