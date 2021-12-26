namespace Morgemil.Data.Convertors

open Microsoft.FSharp.Reflection
open Newtonsoft.Json
open System

type EnumUnionConvertor() =
    inherit JsonConverter()

    override this.CanConvert(t) =
        FSharpType.IsUnion(t)
        && FSharpType.GetUnionCases(t)
           |> Seq.forall
               (fun unionCase ->
                   unionCase.GetFields().Length = 0
                   && unionCase.Name
                      |> System.String.IsNullOrWhiteSpace
                      |> not)

    override this.WriteJson(writer, value, serializer) =
        let value =
            if value = null then
                null
            else
                let case, _ =
                    FSharpValue.GetUnionFields(value, value.GetType())

                case.Name

        serializer.Serialize(writer, value)

    override this.ReadJson(reader, t, existingValue, serializer) =
        let value = serializer.Deserialize(reader)

        if value <> null then
            let case =
                FSharpType.GetUnionCases(t)
                |> Seq.find (fun x -> String.Equals(x.Name, value.ToString(), StringComparison.OrdinalIgnoreCase))

            FSharpValue.MakeUnion(case, [||])
        else
            null
