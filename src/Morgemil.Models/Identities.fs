namespace Morgemil.Models

open System


[<AttributeUsage(AttributeTargets.Class)>]
type RecordAttribute() =
    inherit Attribute()

[<AttributeUsage(AttributeTargets.Property)>]
type RecordIdAttribute() =
    inherit Attribute()

[<AttributeUsage(AttributeTargets.Property)>]
type MeasureByAttribute(name: string) =
    inherit Attribute()
    member this.Name = name

[<Struct>]
type TileFeatureID =
    | TileFeatureID of int64

    member this.Key =
        let (TileFeatureID rowID) = this
        rowID

[<Struct>]
type RaceID =
    | RaceID of int64

    member this.Key =
        let (RaceID rowID) = this
        rowID

[<Struct>]
type RaceModifierID =
    | RaceModifierID of int64

    member this.Key =
        let (RaceModifierID rowID) = this
        rowID

[<Struct>]
type TileID =
    | TileID of int64

    member this.Key =
        let (TileID rowID) = this
        rowID

[<Struct>]
type TileInstanceID =
    | TileInstanceID of int64

    member this.Key =
        let (TileInstanceID rowID) = this
        rowID

[<Struct>]
type CharacterID =
    | CharacterID of int64

    member this.Key =
        let (CharacterID characterID) = this
        characterID

[<Struct>]
type ItemID =
    | ItemID of int64

    member this.Key =
        let (ItemID rowID) = this
        rowID

[<Struct>]
type AspectID =
    | AspectID of int64

    member this.Key =
        let (AspectID rowID) = this
        rowID

[<Struct>]
type FloorGenerationParameterID =
    | FloorGenerationParameterID of int64

    member this.Key =
        let (FloorGenerationParameterID rowID) = this
        rowID

[<Struct>]
type MonsterGenerationParameterID =
    | MonsterGenerationParameterID of int64

    member this.Key =
        let (MonsterGenerationParameterID rowID) = this
        rowID

[<Struct>]
type PlayerID =
    | PlayerID of int64

    member this.Key =
        let (PlayerID rowID) = this
        rowID

[<Measure>]
type TileDistance

[<Measure>]
type Weight

[<Measure>]
type HandSlot

[<Measure>]
type TimeTick

[<Measure>]
type Floor
