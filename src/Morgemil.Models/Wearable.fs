namespace Morgemil.Models

type WearableType =
  | Head
  | Chest
  | Hand
  | Legs
  | Feet
  | Waist
  | Fingers
  | Neck
  | Cloak
  | Shield

type Wearable =
  { ///The weight of this item. Used in stamina
    Weight: decimal<Weight>
    ///Where this wearable resides
    WearableType: WearableType
  }
