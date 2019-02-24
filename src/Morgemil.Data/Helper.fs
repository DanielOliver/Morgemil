module Morgemil.Data.Helper
open FSharp.Data

let optionToResult (errorMessage: string) (value: _ option): Result<_, string> =
    match value with
    | Some x -> Result.Ok x
    | None -> Result.Error errorMessage
    
let resultWithDefault (defaultValue: _) (value: Result<_,string>): _ =
    match value with
    | Ok(x) -> x
    | _ -> defaultValue    

let expectedProperty (propertyName: string) (value: _ option): Result<_, string> =
    optionToResult (sprintf "Expected property %s" propertyName) value    

let tryParseLongProperty(propertyName: string) (value: JsonValue): Result<int64, string> =
    propertyName
    |> value.TryGetProperty
    |> Option.map(fun t -> t.AsInteger64())
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

let tryParseLongPropertyWith(propertyName: string) (asID: int64 -> _) (value: JsonValue): Result<_, string> =
    value
    |> tryParseLongProperty propertyName
    |> Result.map asID
