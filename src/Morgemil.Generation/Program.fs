// Learn more about F# at http://fsharp.org

open System
open System.IO
open Microsoft.FSharp.Reflection

let IsEnumUnion (t: Type) =
    FSharpType.IsUnion(t)
    && FSharpType.GetUnionCases(t)
        |> Seq.forall(fun unionCase ->
            unionCase.GetFields().Length = 0
            && unionCase.Name |> System.String.IsNullOrWhiteSpace |> not
        )

let IndentationLevel = 4

let IsMorgemilType(t: Type) =
    t.Namespace.StartsWith("Morgemil")

let IsSingleCase(t: Type) =
    FSharpType.IsUnion(t)
    && FSharpType.GetUnionCases(t).Length = 1
    && FSharpType.GetUnionCases(t).[0].GetFields().Length = 1

let IsEnumUnionCase(t: Type) =
    FSharpType.IsUnion(t)
    && FSharpType.GetUnionCases(t)
        |> Seq.forall(fun unionCase ->
            unionCase.GetFields().Length = 0
            && unionCase.Name |> System.String.IsNullOrWhiteSpace |> not
        )

let GenerateDtoFromType (t: Type) =
    let typeName = t.Name + "Dto"
    let fields = FSharpType.GetRecordFields(t)

    /// Generic Type * IsOption * IsList
    let innerType (t: Type): (Type * bool * bool) =
        if t.IsGenericType then
            if t.GetGenericTypeDefinition() = typedefof<option<_>> then
                t.GenericTypeArguments.[0], true, false
            else if t.GetGenericTypeDefinition() = typedefof<list<_>> then
                t.GenericTypeArguments.[0], false, true
            else
                failwithf "What is generic type %s" (t.GetGenericTypeDefinition().Name)
        else
            t, false, false

    let sw = new StringWriter()
    let writeIndentation() = sw.Write(String.replicate IndentationLevel " ")
    let writeIndentationLevel(level) = sw.Write(String.replicate (IndentationLevel * level) " ")
    sw.WriteLine(sprintf "type %s =" typeName)
    writeIndentation()
    sw.WriteLine("{")
    for field in fields do
        writeIndentationLevel(2)
        let (innerType, isOption, isList) = innerType field.PropertyType
        let typeModifier =
            if isOption then " option"
            else if isList then " list"
            else ""
        let typeName = innerType.Name + if IsMorgemilType innerType && not (IsSingleCase innerType) && not (IsEnumUnion innerType) then "Dto" else ""
        sw.WriteLine(sprintf "%s: %s%s" field.Name typeName typeModifier)
    writeIndentation()
    sw.WriteLine("}")
    sw.ToString()


[<EntryPoint>]
let main argv =
    let assembly = typeof<Morgemil.Models.Character>.Assembly
    let modelTypes = assembly.ExportedTypes
    let enumUnionTypes = assembly.ExportedTypes |> Seq.filter IsEnumUnion

    let fieldName1 =
        modelTypes
        |> Seq.filter (fun t -> t.GetCustomAttributes( typeof<Morgemil.Models.RecordSerializationAttribute>, true ).Length > 0)
        |> Seq.map GenerateDtoFromType


    File.WriteAllLines("test.fs", fieldName1)

    0 // return an integer exit code
