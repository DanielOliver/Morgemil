namespace Morgemil.Models

[<RequireQualifiedAccess>]
type ItemType =
    | Weapon
    | Wearable
    | Consumable

[<RequireQualifiedAccess>]
type SubItem =
    | Weapon of Weapon
    | Wearable of Wearable
    | Consumable of Consumable

    member this.ItemType =
        match this with
        | Weapon _ -> ItemType.Weapon
        | Wearable _ -> ItemType.Wearable
        | Consumable _ -> ItemType.Consumable

[<Record>]
type Item =
    { [<RecordId>]
      ID: ItemID
      ///The union of items
      SubItem: SubItem
      ///Name of this item
      Noun: string
      ///If true, then never appears more than once in a game.
      IsUnique: bool }

    ///The general classification
    [<System.Text.Json.Serialization.JsonIgnore>]
    member this.ItemType = this.SubItem.ItemType

    interface Relational.IRow with
        [<System.Text.Json.Serialization.JsonIgnore>]
        member this.Key = this.ID.Key
