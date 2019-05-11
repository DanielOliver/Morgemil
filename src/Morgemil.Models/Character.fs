namespace Morgemil.Models

open Morgemil.Math
open Newtonsoft.Json

[<RecordSerialization>]
type Character =
    {
        ID: CharacterID
        Race: Race
        RaceModifier: RaceModifier option
        Position: Vector2i
        PlayerID: PlayerID option
    }
    interface Relational.IRow with
        [<JsonIgnore()>]
        member this.Key = this.ID.Key
