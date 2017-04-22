namespace Morgemil.Models
type RaceModifierRatio =
  { ///If this is None, then "vanilla" race.
    RaceModifierID: int option
    ///The ratio doesn't have to add up to 100.  Every ratio could be thought of "10 to 1" or something like that.
    Ratio: int
  }

