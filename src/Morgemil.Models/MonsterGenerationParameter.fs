namespace Morgemil.Models

open Newtonsoft.Json

[<RecordSerialization>]
type RaceModifierLink =
  { RaceID: RaceID
    RaceModifierID: RaceModifierID option
    ///The ratio doesn't have to add up to 100.  Every ratio could be thought of "10 to 1" or something like that.
    Ratio: int
  }

[<RecordSerialization>]
type MonsterGenerationParameter =
  { ID: MonsterGenerationParameterID
    GenerationRatios: RaceModifierLink list
  }

  interface Relational.IRow with
      [<JsonIgnore()>]
      member this.Key = this.ID.Key

