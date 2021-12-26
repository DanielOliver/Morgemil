namespace Morgemil.Data.Convertors

open Microsoft.FSharp.Reflection
open Newtonsoft.Json

// Convert single case unions like "type Email = Email of string" to/from json.
type SingleCaseUnionConverter() =
    inherit JsonConverter()

    override this.CanConvert(t) =
        FSharpType.IsUnion(t)
        && FSharpType.GetUnionCases(t).Length = 1
        && FSharpType.GetUnionCases(t).[0].GetFields().Length = 1

    override this.WriteJson(writer, value, serializer) =
        let value =
            if value = null then
                null
            else
                let _, fields =
                    FSharpValue.GetUnionFields(value, value.GetType())

                fields.[0]

        serializer.Serialize(writer, value)

    override this.ReadJson(reader, t, existingValue, serializer) =
        let value = serializer.Deserialize(reader)

        if value <> null then
            FSharpValue.MakeUnion(FSharpType.GetUnionCases(t).[0], [| value |])
        else
            null
