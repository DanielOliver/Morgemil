namespace Morgemil.Models

[<Record>]
type GenerationRatio =
    {
        Tags: CharacterTags Set
        ///The ratio doesn't have to add up to 100.  Every ratio could be thought of "10 to 1" or something like that.
        Ratio: int option
        Min: int option
        Max: int option
    }

[<Record>]
type MonsterGenerationParameter =
    { [<RecordId>]
      ID: MonsterGenerationParameterID
      GenerationRatios: GenerationRatio list }

    interface Relational.IRow with
        [<System.Text.Json.Serialization.JsonIgnore>]
        member this.Key = this.ID.Key
