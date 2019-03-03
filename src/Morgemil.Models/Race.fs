namespace Morgemil.Models

type Race =
  { ID : RaceID
    ///Proper noun
    Noun : string
    ///Proper adjective
    Adjective : string
    ///User-visible description
    Description : string
    ///Valid modifiers for this race
    PossibleRaceModifiers: RaceModifier list
  }
  
  interface Relational.IRow with
      member this.Key = this.ID.Key