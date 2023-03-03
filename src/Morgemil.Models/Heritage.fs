namespace Morgemil.Models

[<Record>]
type Heritage =
    {
        [<RecordId>]
        ID: HeritageID
        ///Proper noun
        Noun: string
        ///Proper adjective
        Adjective: string
        ///User-visible description
        Description: string
        ///Tags this heritage has
        Tags: Map<string, MorTags>
        ///Required tags for procedural matching.
        AncestryTags: Map<string, MorTagMatches>
    }

    interface Relational.IRow with
        [<System.Text.Json.Serialization.JsonIgnore>]
        member this.Key = this.ID.Key
