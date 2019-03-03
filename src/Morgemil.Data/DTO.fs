module Morgemil.Data.DTO
open System
open Morgemil.Models.Relational

type Color = 
    {   A: byte
        B: byte
        G: byte
        R: byte
    }
    
type Vector2i = 
  { X: int
    Y: int
  }    
    
type Rectangle = 
  { Position : Vector2i
    Size : Vector2i
  }    
    
type TileRepresentation =
  {    AnsiCharacter: int
       ForegroundColor: Color
       BackGroundColor: Color
  }

type Race = 
  { ID : int64
    ///Proper noun
    Noun : string
    ///Proper adjective
    Adjective : string
    ///User-visible description
    Description : string
    ///Valid race modifiers
    PossibleRaceModifiers : int64 list
  }
  
  interface IRow with
      member this.Key = this.ID

type RaceModifier =
  { ID: int64
    ///Proper noun
    Noun: string
    ///Proper adjective
    Adjective: string
    ///User-visible description
    Description: string }
  
  interface IRow with
      member this.Key = this.ID

type RaceModifierLink =
  { RaceID : int64
    RaceModifierID : Nullable<int64>
    ///The ratio doesn't have to add up to 100.  Every ratio could be thought of "10 to 1" or something like that.
    Ratio : int
  }

type MonsterGenerationParameter =
  { ID : int64
    GenerationRatios: RaceModifierLink list
  }
  
  interface IRow with
      member this.Key = this.ID

[<RequireQualifiedAccess>]
type TileType = 
  | Void = 0
  | Solid = 1
  | Ground = 2

type Tile = 
  { ID : int64
    ///The general specification of this tile
    TileType: TileType
    /// A short name. eg: "Lush Grass"
    Name : string
    ///A long description. eg: "Beware the burning sand. Scorpions and asps make their home here."
    Description : string
    ///If true, this tile ALWAYS blocks ALL movement by ANYTHING.
    BlocksMovement : bool
    ///If true, this tile ALWAYS blocks ALL Line-Of-Sight of ANYTHING.
    BlocksSight : bool
    ///What this tile looks like.
    Representation: TileRepresentation }
  
  interface IRow with
      member this.Key = this.ID

type TileFeature =
    {   ID: int64
        /// A short name. eg: "Stairs Down"
        Name : string
        ///A long description. eg: "Take these stairs down to the next level."
        Description : string
        ///If true, this Tile Feature ALWAYS blocks ALL movement by ANYTHING.
        BlocksMovement : bool
        ///If true, this Tile Feature ALWAYS blocks ALL Line-Of-Sight of ANYTHING.
        BlocksSight : bool
        ///What this tile Feature looks like.
        Representation: TileRepresentation
        ///The tiles that this feature is valid to exist on.
        PossibleTiles: int64 list
    }
  
    interface IRow with
        member this.Key = this.ID

[<RequireQualifiedAccess>]
type WeaponRangeType =
  | Melee = 0
  | Ranged = 1

type Weapon =
  { ///Type of this weapon
    RangeType: WeaponRangeType
    ///Base Range
    BaseRange: int
    ///The number of hands required to wield this weapon
    HandCount: int
    ///The weight of this item. Used in stamina
    Weight: decimal
  }
  
type WearableType =
  | Head = 0
  | Chest = 1
  | Hand = 2
  | Legs = 3
  | Feet = 4
  | Waist = 5
  | Fingers = 6
  | Neck = 7
  | Cloak = 8
  | Shield = 9

type Wearable =
  { ///The weight of this item. Used in stamina
    Weight: decimal
    ///Where this wearable resides
    WearableType: WearableType
  }

type Consumable = 
  { Uses: int 
  }

[<RequireQualifiedAccess>]
type ItemType = 
  | Weapon = 0
  | Wearable = 1
  | Consumable = 2
    
type Item =
  { ID: int64
    ///The union of items
    Weapon: Weapon list
    Wearable: Wearable list
    Consumable: Consumable list
    ///The general classification
    ItemType: ItemType
    ///Name of this item
    Noun: string
    ///If true, then never appears more than once in a game.
    IsUnique: bool
  }
  
  interface IRow with
      member this.Key = this.ID

[<RequireQualifiedAccess>]
type FloorGenerationStrategy = 
  | OpenFloor = 0

type FloorGenerationParameter =
  { ID: int64
    /// Default Tile
    DefaultTile: int64
    ///Tiles used
    Tiles: int64 list
    ///Size generation
    SizeRange: Rectangle
    ///Generation Strategy
    Strategy: FloorGenerationStrategy
  }
  
  interface IRow with
      member this.Key = this.ID
        
  
type DtoValidResult<'T> =
    {   Item: 'T
        Errors: string list
        Success: bool
    }

  
type RawDtoPhase0 =
    {    Tiles: DtoValidResult<Tile[]>
         TileFeatures: DtoValidResult<TileFeature[]>
         Races: DtoValidResult<Race[]>
         RaceModifiers: DtoValidResult<RaceModifier[]>
         MonsterGenerationParameters: DtoValidResult<MonsterGenerationParameter[]>
         Items: DtoValidResult<Item[]>
         FloorGenerationParameters: DtoValidResult<FloorGenerationParameter[]>
    }
    
    member this.Errors: string list =
        [|    this.Tiles.Errors
              this.TileFeatures.Errors
              this.Races.Errors
              this.RaceModifiers.Errors
              this.MonsterGenerationParameters.Errors
              this.Items.Errors
              this.FloorGenerationParameters.Errors
        |] |> List.concat
        
    member this.Success: bool =
        let isOk = function | Ok _ -> true | _ -> false
        [    this.Tiles.Success
             this.TileFeatures.Success
             this.Races.Success
             this.RaceModifiers.Success    
             this.MonsterGenerationParameters.Success
             this.Items.Success
             this.FloorGenerationParameters.Success
        ] |> List.forall id
        
  
type RawDtoPhase1 =
    {    Tiles: DtoValidResult<DtoValidResult<Tile>[]>
         TileFeatures: DtoValidResult<DtoValidResult<TileFeature>[]>
         Races: DtoValidResult<DtoValidResult<Race>[]>
         RaceModifiers: DtoValidResult<DtoValidResult<RaceModifier>[]>
         MonsterGenerationParameters: DtoValidResult<DtoValidResult<MonsterGenerationParameter>[]>
         Items: DtoValidResult<DtoValidResult<Item>[]>
         FloorGenerationParameters: DtoValidResult<DtoValidResult<FloorGenerationParameter>[]>
    }
    
    member this.Errors: string list =
        [|    this.Tiles.Errors
              this.TileFeatures.Errors
              this.Races.Errors
              this.RaceModifiers.Errors
              this.MonsterGenerationParameters.Errors
              this.Items.Errors
              this.FloorGenerationParameters.Errors
        |] |> List.concat
        
    member this.Success: bool =
        let isOk = function | Ok _ -> true | _ -> false
        [    this.Tiles.Success
             this.TileFeatures.Success
             this.Races.Success
             this.RaceModifiers.Success
             this.MonsterGenerationParameters.Success
             this.Items.Success
             this.FloorGenerationParameters.Success
        ] |> List.forall id
  
  
type RawDtoPhase2 =
    {    Tiles: Morgemil.Models.Tile []
         TileFeatures: Morgemil.Models.TileFeature []
         Races: Morgemil.Models.Race []
         RaceModifiers: Morgemil.Models.RaceModifier []
         MonsterGenerationParameters: Morgemil.Models.MonsterGenerationParameter []
         Items: Morgemil.Models.Item []
         FloorGenerationParameters: Morgemil.Models.FloorGenerationParameter []
    }