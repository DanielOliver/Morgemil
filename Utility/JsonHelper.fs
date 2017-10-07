module Morgemil.Utility.JsonHelper

open FSharp.Data.Runtime
open FSharp.Data
open FSharp.Data.HtmlAttribute

let private culture = System.Globalization.CultureInfo.InvariantCulture

[<RequireQualifiedAccess>]
type JsonError =
    | MissingProperty of PropertyName: string * Record: JsonValue
    | InconsistentArray of WrongValues: JsonValue []
    | UnexpectedType of PropertyName: string * JsonValue
    | PropertyErrors of JsonError[]

let AsString jsonValue = JsonConversions.AsString false culture jsonValue
let AsInteger jsonValue = JsonConversions.AsInteger culture jsonValue
let AsInteger64 jsonValue = JsonConversions.AsInteger64 culture jsonValue
let AsDecimal jsonValue = JsonConversions.AsDecimal culture jsonValue
let AsFloat jsonValue = JsonConversions.AsFloat Array.empty false culture jsonValue
let AsBoolean jsonValue = JsonConversions.AsBoolean jsonValue
let AsDateTime jsonValue = JsonConversions.AsDateTime culture jsonValue
let AsGuid jsonValue = JsonConversions.AsGuid jsonValue
let AsEnum<'t when 't : (new: unit -> 't) and 't : struct and 't :> System.ValueType > jsonValue = 
    jsonValue 
    |> AsString 
    |> Option.bind(
        System.Enum.TryParse<'t> 
        >> function
            | true, x -> Some x
            | false, _ -> None)
let AsArray jsonValue = match jsonValue with | JsonValue.Array arr -> Some arr | _ -> None
let ArrayAsType (conversion: JsonValue -> _ option) (value: JsonValue) =
    match value with 
    | JsonValue.Array array ->
        let items = array |> Array.map(fun value ->
            match conversion value with
            | Some x -> value, Some x
            | None -> value, None)
        let errors = items |> Array.choose(fun (value, opt) -> match opt with | Some _ -> None | None -> Some(value))
        match errors.Length with
        | 0 -> Ok (items |> Array.map(fun (value, opt) -> opt.Value))
        | _ -> Error (JsonError.InconsistentArray errors)
    | _ -> Error (JsonError.UnexpectedType("", value))

let PropertiesAsType (conversion: string * JsonValue -> Result<_,_>) (value: JsonValue) =
    let items = value.Properties() |> Array.map(fun (name, value) ->
        match conversion (name, value) with
        | Ok success -> Ok success
        | Error err -> Error err)
    let errors = items |> Array.choose(fun t -> match t with | Ok _ -> None | Error err -> Some(t))
    match errors.Length with
    | 0 -> Ok (items |> Array.choose(function | Ok x -> Some x | Error _ -> None))
    | _ -> Error (JsonError.PropertyErrors (items |> Array.choose(function | Ok _ -> None | Error err -> Some err)))

let Require (name,conversion: JsonValue -> _ option) (jsonValue: JsonValue) =
    match name
        |> jsonValue.TryGetProperty with
    | Some x -> 
        match conversion x with
        | Some final -> Ok final
        | None -> Error (JsonError.UnexpectedType (name, jsonValue))
    | None -> Error (JsonError.MissingProperty (name, jsonValue))

let RequireResult (name,conversion: JsonValue -> Result<_,_>) (jsonValue: JsonValue) =
    match name
        |> jsonValue.TryGetProperty with
    | Some x -> conversion x
    | None -> Error (JsonError.MissingProperty (name, jsonValue))

let Optional (name,conversion: JsonValue -> _ option) (jsonValue: JsonValue) =
    name
    |> jsonValue.TryGetProperty
    |> Option.bind conversion

let OptionalResult (name,conversion: JsonValue -> Result<_,_>) (jsonValue: JsonValue) =
    name
    |> jsonValue.TryGetProperty
    |> Option.bind (conversion >> function | Ok x -> Some x | _ -> None)

type JsonBuilder<'t>(value: JsonValue) =
    member this.Bind(m: _ option, f): _ option = 
        f m

    member this.Bind<'u>(m: Result<'u,JsonError>, f): Result<'t,JsonError> = 
        match m with
        | Ok x -> x |> f
        | Error err -> Error err

    member this.Bind(m: string, f): Result<'t,JsonError> = 
        match m
            |> value.TryGetProperty with
        | Some x -> x |> f
        | None -> Error (JsonError.MissingProperty (m, value))

    member this.Bind(m: string option, f) = 
        m |> Option.bind value.TryGetProperty |> f

    member this.Bind<'u>(m: (string * (JsonValue -> 'u option)), f): Result<'t,JsonError> = 
        let result = Require m value
        match result with
        | Ok x -> f x
        | Error err -> Error err

    member this.Bind<'u>(m: (string * (JsonValue -> Result<'u,JsonError>)), f): Result<'t,JsonError> = 
        let result = RequireResult m value
        match result with
        | Ok x -> f x
        | Error err -> Error err

    member this.Bind<'u>(m: JsonValue -> Result<'u,JsonError>, f): Result<'t,JsonError> = 
        let result = m value
        match result with
        | Ok x -> f x
        | Error err -> Error err
        
    member this.Bind(m: JsonValue -> _ option, f) = 
        value |> m |> f
        
    member this.Return(x) = 
        Ok x

type Doom = {
        ID: int
    }


// make an instance of the workflow 
let json value = new JsonBuilder<_>(value)

