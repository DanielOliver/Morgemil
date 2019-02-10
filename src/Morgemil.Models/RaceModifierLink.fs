namespace Morgemil.Models

type RaceModifierLink =
  { ID: RaceModifierLinkID
    RaceID: RaceID
    RaceModifierID: RaceModifierID option
    ///The ratio doesn't have to add up to 100.  Every ratio could be thought of "10 to 1" or something like that.
    Ratio: int
  }

