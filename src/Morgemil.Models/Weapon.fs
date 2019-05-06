namespace Morgemil.Models

[<RequireQualifiedAccess>]
type WeaponRangeType =
  | Melee
  | Ranged

[<RecordSerialization>]
type Weapon =
  { ///Type of this weapon
    RangeType: WeaponRangeType
    ///Base Range
    BaseRange: int<TileDistance>
    ///The number of hands required to wield this weapon
    HandCount: int<HandSlot>
    ///The weight of this item. Used in stamina
    Weight: decimal<Weight>
  }

