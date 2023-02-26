module Morgemil.Data.JsonSettings

open Morgemil.Data.ContractResolver
open Morgemil.Data.Convertors
open Newtonsoft.Json

let settings = new JsonSerializerSettings()
settings.Converters.Add(new EnumUnionConvertor())
settings.Converters.Add(new SingleCaseUnionConverter())
settings.Converters.Add(new MultipleCaseUnionConverter())
settings.Converters.Add(new OptionConverter())
settings.Formatting <- Formatting.None
settings.ContractResolver <- new RowContractResolver()
