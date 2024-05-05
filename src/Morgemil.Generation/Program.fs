open Morgemil.Generation.Analysis

[<EntryPoint>]
let main argv =
    let assembly = typeof<Morgemil.Models.Character>.Assembly

    assembly.ExportedTypes
    |> Seq.filter IsMorgemilType
    |> Seq.filter HasMorgemilRecordAttribute
    |> Seq.iter (fun t -> printfn $"%A{t}")

    0 // return an integer exit code
