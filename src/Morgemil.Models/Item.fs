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
    
type Item =
  { ID: ItemID
    ///The union of items
    SubItem: SubItem
    ///The general classification
    ItemType: ItemType
    ///Name of this item
    Noun: string
    ///If true, then never appears more than once in a game.
    IsUnique: bool
  }

