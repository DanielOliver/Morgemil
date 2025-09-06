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
    | Attributes of EntityAttributes voption
    | FloorLocation of EntityFloorLocation voption
    | FloorActor of EntityFloorActor voption

/// Assume that all properties in this list belong to the same entity.
/// Creating a subset of properties as the short list of what has been updated/changed should make performance
///   for updating indices better. At the very least, makes everything easier to reason about when debugging.
type EntityPropertyList =
    { Items: EntityProperty list
      EntityID: EntityID }

    interface Relational.IRow with
        [<System.Text.Json.Serialization.JsonIgnore>]
        member this.Key = this.EntityID.Key

/// An entity is a generic bundle of components grouped together in a nice type.
/// An entity usually refers to actionable existence on the TileMap with a location and engine prompts.
/// An entity could mean most anything you like so long you extend the EntityProperties and EntityType.
/// Every component (property) is expected to have an EntityID reference so that an individual property may be updated
///   on its own without requiring a full Entity refresh.
[<Record>]
type Entity =
    { [<RecordId>]
      ID: EntityID
      Attributes: EntityAttributes voption
      FloorLocation: EntityFloorLocation voption
      FloorActor: EntityFloorActor voption }

    interface Relational.IRow with
        [<System.Text.Json.Serialization.JsonIgnore>]
        member this.Key = this.ID.Key

    member this.Properties: EntityPropertyList =
        { EntityPropertyList.EntityID = this.ID
          Items =
            [ (EntityProperty.Attributes this.Attributes)
              (EntityProperty.FloorLocation this.FloorLocation)
              (EntityProperty.FloorActor this.FloorActor) ] }

module Entity =

    let applyProperty (property: EntityProperty) (entity: Entity) : Entity =
        match property with
        | EntityProperty.Attributes entityAttributes ->
            { entity with
                Attributes = entityAttributes }
        | EntityProperty.FloorLocation entityFloorLocation ->
            { entity with
                FloorLocation = entityFloorLocation }
        | EntityProperty.FloorActor entityFloorActor ->
            { entity with
                FloorActor = entityFloorActor }

    let rec applyProperties (properties: EntityProperty list) (entity: Entity) : Entity =
        match properties with
        | [] -> entity
        | head :: tail ->
            let entity = applyProperty head entity
            applyProperties tail entity

    let applyPropertyList (properties: EntityPropertyList) (entity: Entity) : Entity =
        applyProperties properties.Items entity

    let (|FloorActor|_|) (entity: Entity) =
        match entity.FloorActor with
        | ValueNone -> ValueNone
        | ValueSome floorActor -> ValueSome floorActor

    let (|FloorLocation|_|) (entity: Entity) =
        match entity.FloorLocation with
        | ValueNone -> ValueNone
        | ValueSome floorLocation -> ValueSome floorLocation

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
                if old.Attributes <> next.Attributes then
                    yield
                        EntityEvent.UpdatedProperty(
                            old.Attributes |> EntityProperty.Attributes,
                            next.Attributes |> EntityProperty.Attributes
                        )

                if old.FloorActor <> next.FloorActor then
                    yield
                        EntityEvent.UpdatedProperty(
                            old.FloorActor |> EntityProperty.FloorActor,
                            next.FloorActor |> EntityProperty.FloorActor
                        )

                if old.FloorLocation <> next.FloorLocation then
                    yield
                        EntityEvent.UpdatedProperty(
                            old.FloorLocation |> EntityProperty.FloorLocation,
                            next.FloorLocation |> EntityProperty.FloorLocation
                        )
        }
