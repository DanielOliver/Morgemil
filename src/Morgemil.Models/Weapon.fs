namespace Morgemil.Models

[<RequireQualifiedAccess>]
type WeaponRangeType =
  | Melee = 0
  | Ranged = 1

type Weapon =
  { ///Type of this weapon
    RangeType: WeaponRangeType
    ///Base Range
    BaseRange: int
    ///The number of hands required to wield this weapon
    HandCount: int
    ///The weight of this item. Used in stamina
    Weight: decimal
  }

