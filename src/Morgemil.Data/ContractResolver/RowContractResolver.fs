namespace Morgemil.Data.ContractResolver

open Microsoft.FSharp.Reflection
open Morgemil.Data.Convertors
open Morgemil.Models.Relational
open Newtonsoft.Json
open Newtonsoft.Json.Serialization
open System
open System.Reflection
open Morgemil.Models
open Morgemil.Models.Relational

type RowContractResolver() =
    inherit DefaultContractResolver()

    override this.CreateContract(objectType: Type): JsonContract =
        let contract: JsonContract = base.CreateContract(objectType);

        contract

    override this.CreateProperty(member2: MemberInfo, memberSerialization: MemberSerialization): JsonProperty =
        let property: JsonProperty = base.CreateProperty(member2, memberSerialization)

        if Attribute.IsDefined(member2, typeof<RowKeySerializationAttribute>) && member2.DeclaringType.IsAssignableFrom(typeof<IRow>) then
            property.Converter <- new RowKeyConvertor()
        property
