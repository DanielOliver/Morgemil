namespace Morgemil.Core

open Morgemil.Models
open Morgemil.Models.Relational

module EntityTable =
    let private diffProperty old next map =
        seq {
            if old <> next then
                yield (map old, map next)
        }

    let rec private diffEntity (old: Entity) (next: Entity) : (EntityProperty * EntityProperty) list =
        seq {
            yield! diffProperty (old.Attributes) (next.Attributes) EntityProperty.Attributes
            yield! diffProperty (old.FloorActor) (next.FloorActor) EntityProperty.FloorActor

            yield! diffProperty (old.FloorLocation) (next.FloorLocation) EntityProperty.FloorLocation
        }
        |> Seq.toList

    let entityTableEventToStepItems (tableEvent: Entity TableEvent) : StepItem =
        match tableEvent with
        | Added _ -> StepItem.Entity tableEvent
        | Removed _ -> StepItem.Entity tableEvent
        | Updated(oldValue, newValue) ->
            let properties = diffEntity oldValue newValue

            let oldProperties = properties |> List.map fst
            let newProperties = properties |> List.map snd

            TableEvent.Updated(
                { EntityPropertyList.Items = oldProperties
                  EntityID = oldValue.ID },
                { EntityPropertyList.Items = newProperties
                  EntityID = oldValue.ID }
            )
            |> StepItem.EntityProperties

type EntityTable(timeTable: TimeTable) as this =
    inherit Table<Entity, EntityID>(EntityID, (_.Key), EntityTable.entityTableEventToStepItems)
    do this.AddIndex timeTable

    member this.Update(next: EntityPropertyList) =
        (this :> ITable<Entity, EntityID>).MapUpdate next.EntityID (Entity.applyPropertyList next)
        |> ignore

    member this.Update(next: EntityFloorLocation) =
        (this :> ITable<Entity, EntityID>).MapUpdate
            next.ID
            (Entity.applyProperty (EntityProperty.FloorLocation(ValueSome next)))
        |> ignore

    member this.Update(next: EntityFloorActor) =
        (this :> ITable<Entity, EntityID>).MapUpdate
            next.ID
            (Entity.applyProperty (EntityProperty.FloorActor(ValueSome next)))
        |> ignore

    member this.Update(next: Entity) =
        (this :> ITable<Entity, EntityID>).MapUpdate next.ID (fun e -> next) |> ignore

    member this.AddOrUpdate(next: Entity) =
        (this :> ITable<Entity, EntityID>).Add(next)
