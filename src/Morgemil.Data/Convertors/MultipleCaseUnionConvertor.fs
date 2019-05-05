namespace Morgemil.Data.Convertors

open Microsoft.FSharp.Reflection
open Newtonsoft.Json

type MultipleCaseUnionConverter () =
    inherit JsonConverter ()

    override this.CanConvert(t) =
        FSharpType.IsUnion(t)
        && FSharpType.GetUnionCases(t)
            |> Seq.forall(fun unionCase ->
                unionCase.GetFields().Length = 1
                && unionCase.Name |> System.String.IsNullOrWhiteSpace |> not
            )

    override this.WriteJson(writer, value, serializer) =
        if value = null then
            serializer.Serialize(writer, value)
        else
            let case, fields = FSharpValue.GetUnionFields(value, value.GetType())
            writer.WriteStartObject()
            writer.WritePropertyName(case.Name)
            writer.WriteStartArray()
            serializer.Serialize(writer, fields.[0])
            writer.WriteEndArray()
            writer.WriteEndObject()

    override this.ReadJson(reader, t, existingValue, serializer) =
        //TODO
        existingValue
