namespace Morgemil.Models

[<Record>]
type Aspect =
    {
        [<RecordId>]
        ID: AspectID
        ///Proper noun
        Noun: string
        ///Proper adjective
        Adjective: string
        ///User-visible description
        Description: string
    }

    interface Relational.IRow with
        [<System.Text.Json.Serialization.JsonIgnore>]
        member this.Key = this.ID.Key
