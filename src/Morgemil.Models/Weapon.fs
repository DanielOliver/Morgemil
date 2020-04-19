namespace Morgemil.Models

[<RequireQualifiedAccess>]
type WeaponRangeType =
  | Melee
  | Ranged

type Weapon =
  { ///Type of this weapon
    RangeType: WeaponRangeType
    ///Base Range
    [<MeasureBy("TileDistance")>] BaseRange: int<TileDistance>
    ///The number of hands required to wield this weapon
    [<MeasureBy("HandSlot")>] HandCount: int<HandSlot>
    ///The weight of this item. Used in stamina
    [<MeasureBy("Weight")>] Weight: decimal<Weight>
  }

