namespace Morgemil.Models

open Newtonsoft.Json

[<Record>]
type RaceModifier =
  { [<RecordId>] ID: RaceModifierID
    ///Proper noun
    Noun: string
    ///Proper adjective
    Adjective: string
    ///User-visible description
    Description: string
    ///Valid races for this modifier
    PossibleRaces: Race list
  }

    interface Relational.IRow with
      [<JsonIgnore()>]
      member this.Key = this.ID.Key

