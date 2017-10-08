module Morgemil.Utility.JsonHelper

open FSharp.Data

let private culture = System.Globalization.CultureInfo.InvariantCulture


let IsError = function | Ok _ -> false | Error _ -> true
let IsOk = function | Ok _ -> true | Error _ -> false

let ToError = function | Ok _ -> None | Error err -> Some err
let ToOk = function | Ok ok -> Some ok | Error err -> None


[<RequireQualifiedAccess>]
type JsonError =
    | MissingProperty of PropertyName: string * Record: JsonValue
    | InconsistentArray of WrongValues: JsonError []
    | UnexpectedType of ExpectedType: string * JsonValue
    | PropertyErrors of WrongValues: JsonError[]
    
    member private this.GetError() = 
        match this with
        | MissingProperty (propertyName, record) -> sprintf "Property \"%s\" is missing from \"%s\"." propertyName (record.ToString())
        | InconsistentArray wrongValues -> sprintf "Array is inconsistent from these element errors: %s" (System.String.Join(";", wrongValues |> Seq.map(fun t -> t.ToString()) |> Seq.toArray))
        | UnexpectedType (expectedType, value) -> sprintf "Value \"%s\" was expected to be of type \"%s\"." (value.ToString()) expectedType
        | PropertyErrors wrongValues -> sprintf "Properties are inconsistent from these element errors: %s" (System.String.Join(";", wrongValues |> Seq.map(fun t -> t.ToString()) |> Seq.toArray))

    override this.ToString() =
        this.GetError()
    
type JsonResult<'a> = Result<'a,JsonError>

let OptionToResult name jsonValue value = 
    match value with
    | Some x -> Ok x
    | None -> Error (JsonError.UnexpectedType(name, jsonValue))
let JsonAsString jsonValue = FSharp.Data.Runtime.JsonConversions.AsString false culture jsonValue |> OptionToResult "string" jsonValue
let JsonAsInteger jsonValue = FSharp.Data.Runtime.JsonConversions.AsInteger culture jsonValue  |> OptionToResult "int" jsonValue
let JsonAsInteger64 jsonValue = FSharp.Data.Runtime.JsonConversions.AsInteger64 culture jsonValue  |> OptionToResult "int64" jsonValue
let JsonAsDecimal jsonValue = FSharp.Data.Runtime.JsonConversions.AsDecimal culture jsonValue  |> OptionToResult "decimal" jsonValue
let JsonAsFloat jsonValue = FSharp.Data.Runtime.JsonConversions.AsFloat Array.empty false culture jsonValue  |> OptionToResult "float" jsonValue
let JsonAsBoolean jsonValue = FSharp.Data.Runtime.JsonConversions.AsBoolean jsonValue  |> OptionToResult "bool" jsonValue
let JsonAsDateTime jsonValue = FSharp.Data.Runtime.JsonConversions.AsDateTime culture jsonValue  |> OptionToResult "datetime" jsonValue
let JsonAsGuid jsonValue = FSharp.Data.Runtime.JsonConversions.AsGuid jsonValue  |> OptionToResult "guid" jsonValue
let JsonAsEnum<'t when 't : (new: unit -> 't) and 't : struct and 't :> System.ValueType > jsonValue = 
    jsonValue 
    |> JsonAsString
    |> Result.bind(
        System.Enum.TryParse<'t>
        >> function
            | true, x -> Ok x
            | false, _ -> Error (JsonError.UnexpectedType(typeof<'t>.Name, jsonValue)))
let JsonAsArrayI (conversion: int -> JsonValue -> JsonResult<_>) (value: JsonValue) =
    match value with 
    | JsonValue.Array array ->
        let items = array |> Array.mapi(conversion)
        
        let errors = items |> Array.choose(ToError)
        match errors.Length with
        | 0 -> Ok (items |> Array.choose(ToOk))
        | _ -> Error (JsonError.InconsistentArray errors)
    | _ -> Error (JsonError.UnexpectedType("array", value))
let JsonAsArray (conversion: JsonValue -> JsonResult<_>) (value: JsonValue) =
    JsonAsArrayI (fun _ item -> conversion item) value
let JsonAsProperties (conversion: string * JsonValue -> JsonResult<_>) (value: JsonValue) =
    match value with 
    | JsonValue.Record record ->
        let items = record |> Array.map(conversion)
        
        let errors = items |> Array.choose(ToError)
        match errors.Length with
        | 0 -> Ok (items |> Array.choose(ToOk))
        | _ -> Error (JsonError.PropertyErrors errors)
    | _ -> Error (JsonError.UnexpectedType("record", value))


let Require (name,conversion: JsonValue -> JsonResult<_>) (jsonValue: JsonValue) =
    match name
        |> jsonValue.TryGetProperty with
    | Some x -> conversion x
    | None -> Error (JsonError.MissingProperty (name, jsonValue))
    
let Optional (name,conversion: JsonValue -> JsonResult<_>) (jsonValue: JsonValue) =
    name
    |> jsonValue.TryGetProperty
    |> Option.map (conversion)

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
        
    member this.Bind<'u>(m: (string * (JsonValue -> JsonResult<'u>)), f): Result<'t,JsonError> = 
        let result = Require m value
        match result with
        | Ok x -> f x
        | Error err -> Error err

    member this.Bind<'u>(m: JsonValue -> JsonResult<'u>, f): Result<'t,JsonError> = 
        let result = m value
        match result with
        | Ok x -> f x
        | Error err -> Error err
        
    member this.Bind(m: JsonValue -> JsonResult<'u> option, f) = 
        match value |> m with
        | Some x ->
            match x with 
            | Ok y -> Some y |> f
            | Error err -> Error err
        | None -> None |> f
        
    member this.Return(x) = 
        Ok x

// make an instance of the workflow 
let json value = new JsonBuilder<_>(value)

