namespace Morgemil.Models

[<RequireQualifiedAccess>]
type EntityType = | FloorCharacter

type EntityFloorLocation =
    { [<RecordId>]
      ID: EntityID
      FloorID: FloorID
      Position: Morgemil.Math.Point }

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
      PlayerID: PlayerID voption }

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

    static member Zero(entityID: EntityID) =
        { EntityAttributes.ID = entityID
          Ancestry =
            { Ancestry.Noun = ""
              Ancestry.Adjective = ""
              Ancestry.Description = ""
              Ancestry.ID = AncestryID 50L
              Ancestry.Tags = Map.empty
              Ancestry.RequireTags = Map.empty }
          Heritage = []
          Tags = Map.empty }

[<RequireQualifiedAccess>]
type EntityProperty =
    | Attributes of EntityAttributes
    | FloorLocation of EntityFloorLocation
    | FloorActor of EntityFloorActor

    [<System.Text.Json.Serialization.JsonIgnore>]
    member this.EntityID =
        match this with
        | Attributes a -> a.ID
        | FloorActor fa -> fa.ID
        | FloorLocation fl -> fl.ID

    interface Relational.IRow with
        [<System.Text.Json.Serialization.JsonIgnore>]
        member this.Key = this.EntityID.Key

/// Assume that all properties in this list belong to the same entity.
/// Creating a subset of properties as the short list of what has been updated/changed should make performance
///   for updating indices better. At the very least, makes everything easier to reason about when debugging.
type EntityPropertyList =
    | Items of EntityProperty list

    [<System.Text.Json.Serialization.JsonIgnore>]
    member this.ID =
        match this with
        | Items i -> i.Head.EntityID

    interface Relational.IRow with
        [<System.Text.Json.Serialization.JsonIgnore>]
        member this.Key = this.ID.Key

///A player, a NPC, or a monster.
type EntityFloorCharacter =
    { [<RecordId>]
      ID: EntityID
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

module Entity =
    let floorLocation (entity: Entity) : EntityFloorLocation voption =
        match entity.Properties with
        | EntityProperties.FloorCharacter entityFloorCharacter -> entityFloorCharacter.FloorLocation |> ValueSome

    let floorActor (entity: Entity) : EntityFloorActor voption =
        match entity.Properties with
        | EntityProperties.FloorCharacter entityFloorCharacter -> entityFloorCharacter.FloorActor |> ValueSome

    let applyProperty (property: EntityProperty) (entity: Entity) : Entity =
        let floorCharacter x =
            { entity with
                Properties = EntityProperties.FloorCharacter x }

        match entity.Properties with
        | EntityProperties.FloorCharacter entityFloorCharacter ->
            match property with
            | EntityProperty.Attributes entityAttributes ->
                floorCharacter
                    { entityFloorCharacter with
                        Attributes = entityAttributes }
            | EntityProperty.FloorLocation entityFloorLocation ->
                floorCharacter
                    { entityFloorCharacter with
                        FloorLocation = entityFloorLocation }
            | EntityProperty.FloorActor entityFloorActor ->
                floorCharacter
                    { entityFloorCharacter with
                        FloorActor = entityFloorActor }

    let rec applyProperties (properties: EntityProperty list) (entity: Entity) : Entity =
        match properties with
        | [] -> entity
        | head :: tail ->
            let entity = applyProperty head entity
            applyProperties tail entity

    let rec applyPropertyList (properties: EntityPropertyList) (entity: Entity) : Entity =
        match properties with
        | Items entityProperties -> applyProperties entityProperties entity

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
