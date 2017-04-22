namespace Morgemil.Models

type RaceModifierLink =
  { ID: int
    Race: Race
    RaceModifier: RaceModifier option
    ///The ratio doesn't have to add up to 100.  Every ratio could be thought of "10 to 1" or something like that.
    Ratio: int
  }

