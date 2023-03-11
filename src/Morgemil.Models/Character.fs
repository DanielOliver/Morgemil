namespace Morgemil.Models

open Morgemil.Math
open Morgemil.Models

module Character =
    let DefaultTickActions =
        [ ActionArchetype.CharacterBeforeInput
          ActionArchetype.CharacterEngineInput
          ActionArchetype.CharacterAfterInput ]

    let DefaultPlayerTickActions =
        [ ActionArchetype.CharacterBeforeInput
          ActionArchetype.CharacterPlayerInput
          ActionArchetype.CharacterAfterInput ]

[<Record>]
type Character =
    {
        [<RecordId>]
        ID: CharacterID
        Ancestry: Ancestry
        /// Ordered by priority
        Heritage: Heritage list
        Position: Vector2i
        NextTick: int64<TimeTick>
        Floor: int64<Floor>
        NextAction: ActionArchetype
        TickActions: ActionArchetype list
        PlayerID: PlayerID option
        Tags: Map<string, string>
    }

    interface Relational.IRow with
        [<System.Text.Json.Serialization.JsonIgnore>]
        member this.Key = this.ID.Key
