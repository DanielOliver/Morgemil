module Morgemil.Data.Phases

open Morgemil.Data.DTO

/// A Result.Ok and Result.Error combined
type DtoValidResult<'T> =
    { Object: 'T
      Errors: string list
      Success: bool }

type RawDtoPhase0InitialLoad =
    { Tiles: DtoValidResult<Tile[]>
      TileFeatures: DtoValidResult<TileFeature[]>
      Ancestries: DtoValidResult<Ancestry[]>
      Heritages: DtoValidResult<Heritage[]>
      MonsterGenerationParameters: DtoValidResult<MonsterGenerationParameter[]>
      Items: DtoValidResult<Item[]>
      FloorGenerationParameters: DtoValidResult<FloorGenerationParameter[]>
      Aspects: DtoValidResult<Aspect[]>
      Towers: DtoValidResult<Tower[]> }

    member this.Errors: string list =
        [| this.Tiles.Errors
           this.TileFeatures.Errors
           this.Ancestries.Errors
           this.Heritages.Errors
           this.MonsterGenerationParameters.Errors
           this.Items.Errors
           this.FloorGenerationParameters.Errors
           this.Aspects.Errors
           this.Towers.Errors |]
        |> List.concat

    member this.Success: bool =
        [ this.Tiles.Success
          this.TileFeatures.Success
          this.Ancestries.Success
          this.Heritages.Success
          this.MonsterGenerationParameters.Success
          this.Items.Success
          this.FloorGenerationParameters.Success
          this.Aspects.Success
          this.Towers.Success ]
        |> List.forall id

type RawDtoPhase1Validation =
    { Tiles: DtoValidResult<DtoValidResult<Tile>[]>
      TileFeatures: DtoValidResult<DtoValidResult<TileFeature>[]>
      Ancestries: DtoValidResult<DtoValidResult<Ancestry>[]>
      Heritages: DtoValidResult<DtoValidResult<Heritage>[]>
      MonsterGenerationParameters: DtoValidResult<DtoValidResult<MonsterGenerationParameter>[]>
      Items: DtoValidResult<DtoValidResult<Item>[]>
      FloorGenerationParameters: DtoValidResult<DtoValidResult<FloorGenerationParameter>[]>
      Aspects: DtoValidResult<DtoValidResult<Aspect>[]>
      Towers: DtoValidResult<DtoValidResult<Tower>[]> }

    member this.Errors: string list =
        [| this.Tiles.Errors
           this.TileFeatures.Errors
           this.Ancestries.Errors
           this.Heritages.Errors
           this.MonsterGenerationParameters.Errors
           this.Items.Errors
           this.FloorGenerationParameters.Errors
           this.Aspects.Errors
           this.Towers.Errors |]
        |> List.concat

    member this.Success: bool =
        [ this.Tiles.Success
          this.TileFeatures.Success
          this.Ancestries.Success
          this.Heritages.Success
          this.MonsterGenerationParameters.Success
          this.Items.Success
          this.FloorGenerationParameters.Success
          this.Aspects.Success
          this.Towers.Success ]
        |> List.forall id


type RawDtoPhase2Flattened =
    { Tiles: Morgemil.Models.Tile[]
      TileFeatures: Morgemil.Models.TileFeature[]
      Ancestries: Morgemil.Models.Ancestry[]
      Heritages: Morgemil.Models.Heritage[]
      MonsterGenerationParameters: Morgemil.Models.MonsterGenerationParameter[]
      Items: Morgemil.Models.Item[]
      FloorGenerationParameters: Morgemil.Models.FloorGenerationParameter[]
      Aspects: Morgemil.Models.Aspect[]
      Towers: Morgemil.Models.Tower[] }
