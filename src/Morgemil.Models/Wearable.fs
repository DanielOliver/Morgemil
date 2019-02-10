namespace Morgemil.Models

type WearableType =
  | Head = 0
  | Chest = 1
  | Hand = 2
  | Legs = 3
  | Feet = 4
  | Waist = 5
  | Fingers = 6
  | Neck = 7
  | Cloak = 8
  | Shield = 9

type Wearable =
  { ///The weight of this item. Used in stamina
    Weight: decimal<Weight>
    ///Where this wearable resides
    WearableType: WearableType
  }
