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
        Tags: CharacterTags Set
        ///Required tags for procedural matching.
        AncestryTags: CharacterTags Set
    }

    interface Relational.IRow with
        [<System.Text.Json.Serialization.JsonIgnore>]
        member this.Key = this.ID.Key
