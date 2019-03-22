namespace Morgemil.Models
open Morgemil.Math

type Character =
    {
        ID: CharacterID
        Race: Race
        RaceModifier: RaceModifier option
        Position: Vector2i
    }
    interface Relational.IRow with
        member this.Key = this.ID.Key