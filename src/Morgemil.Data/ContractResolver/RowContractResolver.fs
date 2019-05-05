namespace Morgemil.Data.ContractResolver

open Microsoft.FSharp.Reflection
open Morgemil.Data.Convertors
open Morgemil.Models.Relational
open Newtonsoft.Json
open Newtonsoft.Json.Serialization
open System
open System.Reflection
open Morgemil.Models

type RowContractResolver() =
    inherit DefaultContractResolver()

    override this.CreateContract(objectType: Type): JsonContract =
        let contract: JsonContract = base.CreateContract(objectType);

        // this will only be called once and then cached
//        if (objectType == typeof(DateTime) || objectType == typeof(DateTimeOffset))
//        {
//            contract.Converter = new JavaScriptDateTimeConverter();
//        }

        contract

    override this.CreateProperty(member2: MemberInfo, memberSerialization: MemberSerialization): JsonProperty =
        let property: JsonProperty = base.CreateProperty(member2, memberSerialization)

        if property.DeclaringType.IsAssignableFrom(typeof<IRow>) && member2.GetCustomAttributes() |> Seq.exists(fun t -> t.GetType() = typeof<RowKeySerializationAttribute>) then
            property.Converter <- new RowKeyConvertor()

//        if (property.DeclaringType == typeof(Employee) && property.PropertyName == "Manager")
//        {
//            property.ShouldSerialize =
//                instance =>
//                {
//                    Employee e = (Employee)instance;
//                    return e.Manager != e;
//                };
//        }
        property
