// Learn more about F# at http://fsharp.org

open System
open Microsoft.FSharp.Reflection

let IsEnumUnion (t: Type) =
    FSharpType.IsUnion(t)
    && FSharpType.GetUnionCases(t)
        |> Seq.forall(fun unionCase ->
            unionCase.GetFields().Length = 0
            && unionCase.Name |> System.String.IsNullOrWhiteSpace |> not
        )


[<EntryPoint>]
let main argv =
    let assembly = typeof<Morgemil.Models.Character>.Assembly
    let modelTypes = assembly.ExportedTypes
    let enumUnionTypes = assembly.ExportedTypes |> Seq.filter IsEnumUnion

    0 // return an integer exit code
