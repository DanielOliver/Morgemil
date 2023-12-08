module Morgemil.Utility.ResultBuilder

open System

let ofOption error =
    function
    | Some s -> Ok s
    | None -> Error error

// http://www.fssnip.net/7UJ/title/ResultBuilder-Computational-Expression
type ResultBuilder() =
    member __.Return(x) = Ok x

    member __.ReturnFrom(m: Result<_, _>) = m

    member __.Bind(m, f) = Result.bind f m
    member __.Bind((m, error): Option<'T> * 'E, f) = m |> ofOption error |> Result.bind f

    member __.Zero() = None

    member __.Combine(m, f) = Result.bind f m

    member __.Delay(f: unit -> _) = f

    member __.Run(f) = f ()

    member __.TryWith(m, h) =
        try
            __.ReturnFrom(m)
        with e ->
            h e

    member __.TryFinally(m, compensation) =
        try
            __.ReturnFrom(m)
        finally
            compensation ()

    member __.Using(res: #IDisposable, body) =
        __.TryFinally(
            body res,
            fun () ->
                match res with
                | null -> ()
                | disp -> disp.Dispose()
        )

    member __.While(guard, f) =
        if not (guard ()) then
            Ok()
        else
            do f () |> ignore
            __.While(guard, f)

    member __.For(sequence: seq<_>, body) =
        __.Using(sequence.GetEnumerator(), (fun enum -> __.While(enum.MoveNext, __.Delay(fun () -> body enum.Current))))

let result = new ResultBuilder()
