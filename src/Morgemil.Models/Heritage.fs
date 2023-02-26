namespace Morgemil.Models

open Newtonsoft.Json

[<Record>]
type Heritage =
    { [<RecordId>]
      ID: HeritageID
      ///Proper noun
      Noun: string
      ///Proper adjective
      Adjective: string
      ///User-visible description
      Description: string
      ///Valid ancestries for this modifier
      PossibleAncestries: Ancestry list }

    interface Relational.IRow with
        [<JsonIgnore>]
        member this.Key = this.ID.Key
