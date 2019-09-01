namespace Morgemil.Models

open Newtonsoft.Json

[<Record>]
type RaceModifierLink =
  { RaceID: RaceID
    RaceModifierID: RaceModifierID option
    ///The ratio doesn't have to add up to 100.  Every ratio could be thought of "10 to 1" or something like that.
    Ratio: int
  }

[<Record>]
type MonsterGenerationParameter =
  { [<RecordId>]  ID: MonsterGenerationParameterID
    GenerationRatios: RaceModifierLink list
  }

  interface Relational.IRow with
      [<JsonIgnore()>]
      member this.Key = this.ID.Key

