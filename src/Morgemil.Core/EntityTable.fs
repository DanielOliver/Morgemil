namespace Morgemil.Core

open Morgemil.Models
open Morgemil.Models.Relational

module EntityTable =
    let private diffProperty old next map =
        seq {
            if old <> next then
                yield (map old, map next)
        }

    let rec private diffFloorCharacter
        (oldFloorCharacter: EntityFloorCharacter)
        (newFloorCharacter: EntityFloorCharacter)
        : (EntityProperty * EntityProperty) list =
        seq {
            yield! diffProperty (oldFloorCharacter.Attributes) (newFloorCharacter.Attributes) EntityProperty.Attributes
            yield! diffProperty (oldFloorCharacter.FloorActor) (newFloorCharacter.FloorActor) EntityProperty.FloorActor

            yield!
                diffProperty
                    (oldFloorCharacter.FloorLocation)
                    (newFloorCharacter.FloorLocation)
                    EntityProperty.FloorLocation
        }
        |> Seq.toList

    let entityTableEventToStepItems (tableEvent: Entity TableEvent) : StepItem =
        match tableEvent with
        | Added _ -> StepItem.Entity tableEvent
        | Removed _ -> StepItem.Entity tableEvent
        | Updated(oldValue, newValue) ->
            match oldValue.Properties, newValue.Properties with
            | EntityProperties.FloorCharacter oldFloorCharacter, EntityProperties.FloorCharacter newFloorCharacter ->
                let properties = diffFloorCharacter oldFloorCharacter newFloorCharacter

                let oldProperties = properties |> List.map fst
                let newProperties = properties |> List.map snd

                TableEvent.Updated(EntityPropertyList.Items oldProperties, EntityPropertyList.Items newProperties)
                |> StepItem.EntityProperties


type EntityTable(timeTable: TimeTable) as this =
    inherit Table<Entity, EntityID>(EntityID, (_.Key), EntityTable.entityTableEventToStepItems)
    do this.AddIndex timeTable

    member this.Update(next: EntityPropertyList) =
        (this :> ITable<Entity, EntityID>).MapUpdate next.ID (Entity.applyPropertyList next)
        |> ignore

    member this.Update(next: EntityFloorLocation) =
        (this :> ITable<Entity, EntityID>).MapUpdate next.ID (Entity.applyProperty (EntityProperty.FloorLocation next))
        |> ignore

    member this.Update(next: EntityFloorActor) =
        (this :> ITable<Entity, EntityID>).MapUpdate next.ID (Entity.applyProperty (EntityProperty.FloorActor next))
        |> ignore

    member this.Update(next: EntityFloorCharacter) =
        (this :> ITable<Entity, EntityID>).MapUpdate next.ID (fun e ->
            { e with
                Properties = EntityProperties.FloorCharacter next })
        |> ignore

    member this.AddOrUpdate(next: EntityFloorCharacter) =
        (this :> ITable<Entity, EntityID>)
            .Add(
                { Entity.ID = next.ID
                  Type = EntityType.FloorCharacter
                  Properties = EntityProperties.FloorCharacter next }
            )
