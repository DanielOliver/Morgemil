namespace Morgemil.Models

[<Record>]
type Ancestry =
    { [<RecordId>]
      ID: AncestryID
      ///Proper noun. eg: "minatour"
      Noun: string
      ///Proper adjective
      Adjective: string
      ///User-visible description
      Description: string
      ///Tags this ancestry has
      Tags: CharacterTags Set
      ///Required tags for procedural matching.
      HeritageTags: CharacterTags Set }

    interface Relational.IRow with
        [<System.Text.Json.Serialization.JsonIgnore>]
        member this.Key = this.ID.Key
