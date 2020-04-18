
open System.IO
open Morgemil.Generation

[<EntryPoint>]
let main argv =
    let assembly = typeof<Morgemil.Models.Character>.Assembly

    assembly.ExportedTypes
    |> Seq.filter Creation.IsMorgemilType
    |> Seq.filter Creation.HasMorgemilRecordAttribute
    |> Seq.iter (fun t ->
        printfn "%A" t
        let analysis = Creation.AnalyzeType t
        printfn "%A" analysis
        File.WriteAllText(sprintf "%s.txt" (t.Name.Trim('"')), sprintf "%A" analysis)

        printfn ""
    )


    0 // return an integer exit code
