namespace Morgemil.Models

///Each array should sorted by the ID.
type ScenarioData =
  { Races: Race []
    Tiles: Tile []
    Items: Item []
    RaceModifiers: RaceModifier []
    FloorGenerationParameters: FloorGenerationParameter []
  }
