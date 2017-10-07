module Morgemil.Utility.JsonHelper

open FSharp.Data.Runtime
open FSharp.Data
open FSharp.Data.HtmlAttribute

let private culture = System.Globalization.CultureInfo.InvariantCulture

[<RequireQualifiedAccess>]
type JsonError =
    | MissingProperty of PropertyName: string
    | InconsistentArray of WrongValues: JsonValue []

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
let ArrayAsType (conversion: JsonValue -> _ option) (array: JsonValue[]) =
    let items = array |> Array.map(fun value ->
        match conversion value with
        | Some x -> value, Some x
        | None -> value, None)
    let errors = items |> Array.choose(fun (value, opt) -> match opt with | Some _ -> None | None -> Some(value))
    match errors.Length with
    | 0 -> Ok (items |> Array.map(fun (value, opt) -> opt.Value))
    | _ -> Error (JsonError.InconsistentArray errors)

let Require (name,conversion) (jsonValue: JsonValue) =
    match name
        |> jsonValue.TryGetProperty
        |> Option.bind conversion with
    | Some x -> Ok x
    | None -> Error (JsonError.MissingProperty name)

let Optional (name,conversion) (jsonValue: JsonValue) =
    name
    |> jsonValue.TryGetProperty
    |> Option.bind conversion

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
        | None -> Error (JsonError.MissingProperty m)

    member this.Bind(m: string option, f) = 
        m |> Option.bind value.TryGetProperty |> f

    member this.Bind<'u>(m: (string * (JsonValue -> 'u option)), f): Result<'t,JsonError> = 
        let result = Require m value
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

let inputValue = JsonValue.String("one")
let outPutResult = 
    json inputValue {
        let! one = ("one", AsString)
        let! nextajskdfj = Optional("one", AsString)
        let! boom = Optional("one", AsInteger)
        
        //let! three = Optional "one" AsString
        let! nextOne = 
            json inputValue {
                let asfd = 5
                let! nine = ""
                return { Doom.ID = 5 }
            }
        printfn "%A" one
        let final = { Doom.ID = 5 }
        return final
    }


