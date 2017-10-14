namespace Morgemil.Models

type ScenarioData =
  { Races: Map<int,Race>
    Tiles: Map<int,Tile>
    Items: Map<int,Item>
    RaceModifiers: Map<int,RaceModifier>
    RaceModifierLinks: Map<int,RaceModifierLink>
    FloorGenerationParameters: Map<int,FloorGenerationParameter>
    Scenario: Scenario
  }
