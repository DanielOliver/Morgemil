namespace Morgemil.Data.Convertors

open System
open Microsoft.FSharp.Reflection
open Newtonsoft.Json
open Newtonsoft.Json.Linq

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
        let jsonObject = JObject.Load(reader);
        let cases = FSharpType.GetUnionCases(t)
        jsonObject.Properties()
        |> Seq.filter(fun property ->
                  property.HasValues
                  && cases |> Seq.exists(fun t -> t.Name.Equals(property.Name, StringComparison.OrdinalIgnoreCase))
                  )
        |> Seq.choose(fun property ->
            let array = jsonObject.[property.Name] :?> JArray
            let case = cases |> Seq.find(fun t -> t.Name.Equals(property.Name, StringComparison.OrdinalIgnoreCase))
            if array.HasValues then
                let firstField = case.GetFields().[0]
                let firstItem = array.[0] :?> JObject                
                let castedItem = firstItem.ToObject(firstField.PropertyType, serializer)
                FSharpValue.MakeUnion(case,[|castedItem|])
                |> Some
            else None
            )
        |> Seq.head
        
