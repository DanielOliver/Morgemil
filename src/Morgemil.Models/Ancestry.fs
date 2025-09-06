namespace Morgemil.Models.Dto

open Morgemil.Models

type Ancestry =
    { ID: int64
      Noun: string
      Adjective: string
      Description: string
      Tags: MorTags list
      RequireTags: Map<string, MorTagMatches> }

    interface Relational.IRow with
        [<System.Text.Json.Serialization.JsonIgnore>]
        member this.Key = this.ID

namespace Morgemil.Models

/// Every inhabitant has a single ancestry that describes common and base attributes.
/// Carnivorous Mold is one example of an ancestry.
[<Record>]
[<ScenarioStaticData>]
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
        Tags: Map<string, MorTags>
        ///Required tags for procedural matching.
        RequireTags: Map<string, MorTagMatches>
    }

    interface Relational.IRow with
        [<System.Text.Json.Serialization.JsonIgnore>]
        member this.Key = this.ID.Key

module Ancestry =
    let toDto (x: Ancestry) : Morgemil.Models.Dto.Ancestry =
        { ID = x.ID.Key
          Noun = x.Noun
          Adjective = x.Adjective
          Description = x.Description
          Tags = x.Tags
          RequireTags = x.RequireTags }
