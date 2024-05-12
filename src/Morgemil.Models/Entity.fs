namespace Morgemil.Models

[<RequireQualifiedAccess>]
type EntityType = | FloorCharacter

type EntityFloorLocation =
    { [<RecordId>]
      ID: EntityID
      FloorID: FloorID
      Location: Morgemil.Math.Point }

    interface Relational.IRow with
        [<System.Text.Json.Serialization.JsonIgnore>]
        member this.Key = this.ID.Key

[<Record>]
type EntityFloorActor =
    { [<RecordId>]
      ID: EntityID
      NextTick: int64<TimeTick>
      NextAction: ActionArchetype
      TickActions: ActionArchetype list
      PlayerID: PlayerID option }

    interface Relational.IRow with
        [<System.Text.Json.Serialization.JsonIgnore>]
        member this.Key = this.ID.Key

type EntityAttributes =
    {
        [<RecordId>]
        ID: EntityID
        Ancestry: Ancestry
        /// Ordered by priority
        Heritage: Heritage list
        Tags: Map<string, string>
    }

    interface Relational.IRow with
        [<System.Text.Json.Serialization.JsonIgnore>]
        member this.Key = this.ID.Key

[<RequireQualifiedAccess>]
type EntityProperty =
    | Attributes of EntityAttributes
    | FloorLocation of EntityFloorLocation
    | FloorActor of EntityFloorActor

    interface Relational.IRow with
        [<System.Text.Json.Serialization.JsonIgnore>]
        member this.Key =
            match this with
            | Attributes a -> a.ID.Key
            | FloorActor fa -> fa.ID.Key
            | FloorLocation fl -> fl.ID.Key

type EntityPropertyList =
    | Items of EntityProperty list

    interface Relational.IRow with
        [<System.Text.Json.Serialization.JsonIgnore>]
        member this.Key =
            match this with
            | Items i -> (i.Head :> Relational.IRow).Key

type EntityFloorCharacter =
    { [<RecordId>]
      EntityID: EntityID
      Attributes: EntityAttributes
      FloorLocation: EntityFloorLocation
      FloorActor: EntityFloorActor }

[<RequireQualifiedAccess>]
type EntityProperties =
    | FloorCharacter of EntityFloorCharacter

    [<System.Text.Json.Serialization.JsonIgnore>]
    member this.EntityType =
        match this with
        | FloorCharacter _ -> EntityType.FloorCharacter

/// An entity is a generic bundle of components grouped together in a nice type.
/// An entity usually refers to actionable existence on the TileMap with a location and engine prompts.
/// An entity could mean most anything you like so long you extend the EntityProperties and EntityType.
/// Every component (property) is expected to have an EntityID reference so that an individual property may be updated
///   on its own without requiring a full Entity refresh.
[<Record>]
type Entity =
    { [<RecordId>]
      ID: EntityID
      Type: EntityType
      Properties: EntityProperties }

    interface Relational.IRow with
        [<System.Text.Json.Serialization.JsonIgnore>]
        member this.Key = this.ID.Key

[<RequireQualifiedAccess>]
type EntityEventType =
    | Added
    | Updated
    | Removed

[<RequireQualifiedAccess>]
type EntityEvent =
    | Added of newValue: Entity
    | Updated of oldValue: Entity * newValue: Entity
    | UpdatedProperty of oldValue: EntityProperty * newValue: EntityProperty
    | Removed of oldValue: Entity

    member this.EntityEventType =
        match this with
        | Added _ -> EntityEventType.Added
        | Removed _ -> EntityEventType.Removed
        | Updated _
        | UpdatedProperty _ -> EntityEventType.Updated

    static member from (oldEntity: Entity voption) (nextEntity: Entity voption) : EntityEvent seq =
        seq {
            match oldEntity, nextEntity with
            | ValueNone, ValueNone -> ()
            | ValueSome old, ValueNone -> yield EntityEvent.Removed old
            | ValueNone, ValueSome next -> yield EntityEvent.Added next
            | ValueSome old, ValueSome next ->
                match old.Properties, next.Properties with
                | EntityProperties.FloorCharacter oldFloorCharacter, EntityProperties.FloorCharacter nextFloorCharacter ->
                    if oldFloorCharacter.Attributes <> nextFloorCharacter.Attributes then
                        yield
                            EntityEvent.UpdatedProperty(
                                oldFloorCharacter.Attributes |> EntityProperty.Attributes,
                                nextFloorCharacter.Attributes |> EntityProperty.Attributes
                            )

                    if oldFloorCharacter.FloorActor <> nextFloorCharacter.FloorActor then
                        yield
                            EntityEvent.UpdatedProperty(
                                oldFloorCharacter.FloorActor |> EntityProperty.FloorActor,
                                nextFloorCharacter.FloorActor |> EntityProperty.FloorActor
                            )

                    if oldFloorCharacter.FloorLocation <> nextFloorCharacter.FloorLocation then
                        yield
                            EntityEvent.UpdatedProperty(
                                oldFloorCharacter.FloorLocation |> EntityProperty.FloorLocation,
                                nextFloorCharacter.FloorLocation |> EntityProperty.FloorLocation
                            )
        }
