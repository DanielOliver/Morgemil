namespace Morgemil.Models

[<Record>]
type Ancestry =
    {
        [<RecordId>]
        ID: AncestryID
        ///Proper noun. eg: "minatour"
        Noun: string
        ///Proper adjective
        Adjective: string
        ///User-visible description
        Description: string
        ///Tags this ancestry has
        Tags: Map<string, string>
        ///Required tags for procedural matching.
        HeritageTags: Map<string, bool>
    }

    interface Relational.IRow with
        [<System.Text.Json.Serialization.JsonIgnore>]
        member this.Key = this.ID.Key
