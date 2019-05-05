namespace Morgemil.Models

open Newtonsoft.Json

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
    [<JsonIgnore()>]
    member this.Key = this.ID.Key