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
    { [<RecordId>]
      ID: CharacterID
      Position: Point
      NextTick: int64<TimeTick>
      FloorID: FloorID
      NextAction: ActionArchetype
      TickActions: ActionArchetype list
      PlayerID: PlayerID option }

    interface Relational.IRow with
        [<System.Text.Json.Serialization.JsonIgnore>]
        member this.Key = this.ID.Key
