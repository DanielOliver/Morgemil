namespace Morgemil.Models

open Morgemil.Math
open Morgemil.Models
open Newtonsoft.Json


[<Record>]
type Character =
    {
        [<RecordId>] ID: CharacterID
        Race: Race
        RaceModifier: RaceModifier option
        Position: Vector2i
        NextTick: int64<TimeTick>
        PlayerID: PlayerID option
    }
    interface Relational.IRow with
        [<JsonIgnore()>]
        member this.Key = this.ID.Key
