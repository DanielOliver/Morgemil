namespace Morgemil.Models

type CharacterAttributes =
    {
        [<RecordId>]
        ID: CharacterID
        Ancestry: Ancestry
        /// Ordered by priority
        Heritage: Heritage list
        Tags: Map<string, string>
    }

    interface Relational.IRow with
        [<System.Text.Json.Serialization.JsonIgnore>]
        member this.Key = this.ID.Key
