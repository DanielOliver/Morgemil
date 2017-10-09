namespace Morgemil.Utility

open System.Runtime.CompilerServices

[<Extension>]
type ResultExtensions =
    
    [<Extension>]
    static member IsOk(x) =
        match x with
        | Result.Ok _ -> true
        | _ -> false
    
    [<Extension>]
    static member IsError(x) =
        match x with
        | Result.Error _ -> true
        | _ -> false
        
    [<Extension>]
    static member GetValue(x) =
        match x with
        | Result.Ok value -> value
        | _ -> failwith "No value found."
        
    [<Extension>]
    static member GetError(x) =
        match x with
        | Result.Error err -> err
        | _ -> failwith "No error found."
        
    [<Extension>]
    static member TryGetValue(x) =
        match x with
        | Result.Ok value -> Some value
        | _ -> None
        
    [<Extension>]
    static member TryGetError(x) =
        match x with
        | Result.Error err -> Some err
        | _ -> None
