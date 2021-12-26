namespace Morgemil.Data.Convertors

open Morgemil.Models.Relational
open Newtonsoft.Json

type RowKeyConvertor() =
    inherit JsonConverter()

    override x.CanConvert(t) = t.IsAssignableFrom(typeof<IRow>)

    override x.WriteJson(writer, value, serializer) =
        let value =
            if value = null then
                0L
            else
                (value :?> IRow).Key

        serializer.Serialize(writer, value)

    override x.ReadJson(reader, t, existingValue, serializer) =
        //TODO
        null
