namespace Morgemil.Models

open Newtonsoft.Json

[<Record>]
type Aspect =
    { [<RecordId>]
      ID: AspectID
      ///Proper noun
      Noun: string
      ///Proper adjective
      Adjective: string
      ///User-visible description
      Description: string }

    interface Relational.IRow with
        [<JsonIgnore>]
        member this.Key = this.ID.Key