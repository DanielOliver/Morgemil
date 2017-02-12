namespace Morgemil.Models

type WeaponRangeType =
  | Melee = 0
  | Ranged = 1

type Weapon =
  { ID: int
    ///Name of this weapon type
    Noun: string
    ///Type of this weapon
    RangeType: WeaponRangeType
    ///The number of hands required to wield this weapon
    HandCount: int
    ///A list of Tags that this Weapon possesses, along with the Value(s)
    Tags : Map<Tag, TagValue>    
  }

