namespace Morgemil.Models

open Newtonsoft.Json

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

[<RecordSerialization>]
type Item =
    {
        ID : ItemID
        ///The union of items
        SubItem : SubItem
        ///Name of this item
        Noun : string
        ///If true, then never appears more than once in a game.
        IsUnique : bool
    }

    ///The general classification
    [<JsonIgnore()>]
    member this.ItemType =
        this.SubItem.ItemType

    interface Relational.IRow with
        [<JsonIgnore()>]
        member this.Key = this.ID.Key
