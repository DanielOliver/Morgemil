namespace Morgemil.Models

open Morgemil.Models
open Morgemil.Models.Relational

type ScenarioData =
    { Ancestries: IReadonlyTable<Ancestry, AncestryID>
      Tiles: IReadonlyTable<Tile, TileID>
      TileFeatures: IReadonlyTable<TileFeature, TileFeatureID>
      Items: IReadonlyTable<Item, ItemID>
      Heritages: IReadonlyTable<Heritage, HeritageID>
      MonsterGenerationParameters: IReadonlyTable<MonsterGenerationParameter, MonsterGenerationParameterID>
      FloorGenerationParameters: IReadonlyTable<FloorGenerationParameter, FloorGenerationParameterID>
      Aspects: IReadonlyTable<Aspect, AspectID>
      Towers: IReadonlyTable<Tower, TowerID> }
