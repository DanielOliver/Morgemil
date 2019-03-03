namespace Morgemil.Models

type [<Struct>] TileFeatureID =
    | TileFeatureID of int64

type [<Struct>] RaceID =
    | RaceID of int64

type [<Struct>] RaceModifierID =
    | RaceModifierID of int64
    
type [<Struct>] RaceModifierLinkID =
    | RaceModifierLinkID of int64

type [<Struct>] TileID =
    | TileID of int64

type [<Struct>] ItemID =
    | ItemID of int64

type [<Struct>] FloorGenerationParameterID =
    | FloorGenerationParameterID of int64
    
type [<Struct>] MonsterGenerationParameterID =
    | MonsterGenerationParameterID of int64

[<Measure>]
type TileDistance

[<Measure>]
type Weight

[<Measure>]
type HandSlot
