namespace Morgemil.Models

/// Every inhabitant may have zero or more pieces of heritage.
/// A heritage is a collection of attributes that modify and affect a creature for good or for ill.
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
        RequireTags: Map<string, MorTagMatches>
    }

    interface Relational.IRow with
        [<System.Text.Json.Serialization.JsonIgnore>]
        member this.Key = this.ID.Key
