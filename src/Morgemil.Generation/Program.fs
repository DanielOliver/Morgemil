open System.Collections.Generic
open System.IO
open Morgemil.Generation
open Morgemil.Generation.Analysis



let indentationLevel = 4

let printType (writer: TextWriter) (t: AstRecordType) =
    let indent (level: int) =
        fprintf writer "%s" (String.replicate (indentationLevel * level) " ")

    writer.WriteLine()
    fprintfn writer "type %sDto =" t.ActualType.Name
    indent 1
    fprintfn writer "{"

    for (_, field) in t.Fields |> Map.toSeq do

        indent 2
        fprintfn writer "%s: %s" field.FieldName "int?"

    indent 1
    fprintfn writer "}"


[<EntryPoint>]
let main argv =
    let assembly = typeof<Morgemil.Models.Character>.Assembly

    let dictionary = new Dictionary<string, DependencyGraph>()

    assembly.ExportedTypes
    |> Seq.filter IsMorgemilType
    |> Seq.filter HasMorgemilRecordAttribute
    |> Seq.iter (fun t ->
        printfn "%A" t
        let analysis = AnalyzeType t

        use writer = new StringWriter()

        match analysis with
        | AstCollectedType.MorgemilRecord r -> Creation.printType writer r
        | _ -> failwithf "???"

        ReadDependencyGraph analysis dictionary

        File.WriteAllText(sprintf "%s.txt" t.Name, writer.ToString()))

    let graph = dictionary |> Seq.map (fun t -> (t.Key, t.Value))

    File.WriteAllText("graph.txt", (sprintf "%A" graph))

    0 // return an integer exit code
