namespace Morgemil.Models

type Race =
  { ID : RaceID
    ///Proper noun
    Noun : string
    ///Proper adjective
    Adjective : string
    ///User-visible description
    Description : string
  }
  
  interface Relational.IRow with
      member this.Key = this.ID.Key