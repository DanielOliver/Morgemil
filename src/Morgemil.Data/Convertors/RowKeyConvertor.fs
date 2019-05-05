namespace Morgemil.Data.Convertors

open System
open Microsoft.FSharp.Reflection
open Morgemil.Models.Relational
open Newtonsoft.Json

type RowKeyConvertor() =
    inherit JsonConverter()
    
    override x.CanConvert(t) = 
        t.IsGenericType && t.IsAssignableFrom(typeof<IRow>)

    override x.WriteJson(writer, value, serializer) =
        let value =
            if value = null then
                0L
            else
                (value :?> IRow).Key
        serializer.Serialize(writer, value)

    override x.ReadJson(reader, t, existingValue, serializer) =
        let innerType = t.GetGenericArguments().[0]
        let innerType = 
            if innerType.IsValueType then (typedefof<Nullable<_>>).MakeGenericType([|innerType|])
            else innerType        
        let value = serializer.Deserialize(reader, innerType)
        let cases = FSharpType.GetUnionCases(t)
        if value = null then FSharpValue.MakeUnion(cases.[0], [||])
        else FSharpValue.MakeUnion(cases.[1], [|value|])


