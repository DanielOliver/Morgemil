namespace Morgemil.Models

[<RequireQualifiedAccess>]
type ItemType = 
  | Weapon = 0
  | Wearable = 1

[<RequireQualifiedAccess>]
type SubItem =
  | Weapon of Weapon
  | Wearable of Wearable
  
  member this.ItemType =
    match this with
    | Weapon _ -> ItemType.Weapon
    | Wearable _ -> ItemType.Wearable
    
type Item =
  { ID: int
    ///The union of items
    SubItem: SubItem
    ///The general classification
    ItemType: ItemType
    ///Name of this item
    Noun: string
    ///If true, then never appears more than once in a game.
    IsUnique: bool
    ///A list of Tags that this Item possesses, along with the Value(s)
    Tags : Map<TagType, Tag>    
  }

