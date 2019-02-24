module Morgemil.Data.Helper
open FSharp.Data

let optionToResult (errorMessage: string) (value: _ option): Result<_, string> =
    match value with
    | Some x -> Result.Ok x
    | None -> Result.Error errorMessage
    
let resultToOption (value: Result<_, string>): _ option  =
    match value with
    | Ok x -> Some x 
    | Error _ -> None
    
let resultWithDefault (defaultValue: _) (value: Result<_,string>): _ =
    match value with
    | Ok(x) -> x
    | _ -> defaultValue

let expectedProperty (propertyName: string) (value: _ option): Result<_, string> =
    optionToResult (sprintf "Expected property %s" propertyName) value
    
let tryParseRecord(propertyName: string) (binder: JsonValue -> Result<_, string>) (value: JsonValue): Result<_, string> =
    propertyName
    |> value.TryGetProperty
    |> expectedProperty propertyName
    |> Result.bind binder

let tryParseLongProperty(propertyName: string) (value: JsonValue): Result<int64, string> =
    propertyName
    |> value.TryGetProperty
    |> Option.bind(fun t ->
        match t with
        | JsonValue.Number _ -> t.AsInteger64() |> Some
        | JsonValue.String x when x |> Seq.forall System.Char.IsDigit -> t.AsInteger64() |> Some
        | _ -> None )
    |> expectedProperty propertyName

let tryParseIntProperty(propertyName: string) (value: JsonValue): Result<int, string> =
    propertyName
    |> value.TryGetProperty
    |> Option.map(fun t -> t.AsInteger())
    |> expectedProperty propertyName    
    
let tryParseCharProperty(propertyName: string) (value: JsonValue): Result<char, string> =
    propertyName
    |> value.TryGetProperty
    |> function
    | Some t ->
        match t with
        | JsonValue.String data ->
            match data with
            | x when x.Length = 0 -> 
                Result.Error (sprintf "Expected property %s to be a non-empty string" propertyName)
            | x ->
                x.[0] |> Result.Ok
        | JsonValue.Number data ->
            tryParseIntProperty propertyName value
            |> Result.map char
        | _ ->
            Result.Error (sprintf "Expected property %s to be a number or string" propertyName)
    | None -> Result.Error (sprintf "Expected property %s" propertyName)


let tryParseStringProperty(propertyName: string) (value: JsonValue): Result<string, string> =
    propertyName
    |> value.TryGetProperty
    |> Option.bind(fun t ->
        match t with
        | JsonValue.String x -> x  |> Some
        | _ -> None )
    |> expectedProperty propertyName

let tryParseLongPropertyWith(propertyName: string) (asID: int64 -> _) (value: JsonValue): Result<_, string> =
    value
    |> tryParseLongProperty propertyName
    |> Result.map asID
