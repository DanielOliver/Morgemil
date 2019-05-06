// Learn more about F# at http://fsharp.org

open System
open System.IO
open System.Reflection
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

let GetSingleCaseType(t: Type) =
    FSharpType.GetUnionCases(t).[0].GetFields().[0].PropertyType

let IsEnumUnionCase(t: Type) =
    FSharpType.IsUnion(t)
    && FSharpType.GetUnionCases(t)
        |> Seq.forall(fun unionCase ->
            unionCase.GetFields().Length = 0
            && unionCase.Name |> System.String.IsNullOrWhiteSpace |> not
        )

let IsMultipleUnionCase(t: Type) =
    FSharpType.IsUnion(t)
    && FSharpType.GetUnionCases(t)
        |> Seq.forall(fun unionCase ->
            unionCase.GetFields().Length = 1
            && unionCase.Name |> System.String.IsNullOrWhiteSpace |> not
        )
    && FSharpType.GetUnionCases(t).Length > 1

let WriteIndentation (sw: StringWriter) = sw.Write(String.replicate IndentationLevel " ")
let WriteIndentationLevel (level) (sw: StringWriter) = sw.Write(String.replicate (IndentationLevel * level) " ")


/// Generic Type * IsOption * IsList
let InnerType (t: Type): (Type * bool * bool) =
    if t.IsGenericType then
        if t.GetGenericTypeDefinition() = typedefof<option<_>> then
            t.GenericTypeArguments.[0], true, false
        else if t.GetGenericTypeDefinition() = typedefof<list<_>> then
            t.GenericTypeArguments.[0], false, true
        else
            failwithf "What is generic type %s" (t.GetGenericTypeDefinition().Name)
    else
        t, false, false

let IsInTypes (currentType: Type) (knownTypes: System.Collections.Generic.List<string>) =
    knownTypes.Contains(currentType.FullName)

let IsRowKey (currentType: Type) =
    typeof<Morgemil.Models.Relational.IRow>.IsAssignableFrom(currentType)

let ShouldAddType (currentType: Type) (knownTypes: System.Collections.Generic.List<string>) =
    IsMorgemilType currentType && not (IsInTypes currentType knownTypes) && not (IsEnumUnion currentType)

let rec WriteDto (writer: StringWriter) (currentType: Type) (knownTypes: System.Collections.Generic.List<string>) =
    if not (ShouldAddType currentType knownTypes) then ()
    else
    knownTypes.Add currentType.FullName

    let typeName = currentType.Name + "Dto"
    let recordFields = FSharpType.GetRecordFields currentType

    for field in recordFields do
        let fieldType = field.PropertyType
        if ShouldAddType fieldType knownTypes then
            if IsMultipleUnionCase fieldType then
                FSharpType.GetUnionCases(fieldType)
                |> Seq.iter(fun case ->
                    let fieldType = case.GetFields().[0].PropertyType
                    WriteDto writer fieldType knownTypes
                    )
            else if not (IsSingleCase fieldType) then
                WriteDto writer fieldType knownTypes
            else
                ()

    writer.WriteLine(sprintf "type %s =" typeName)
    WriteIndentation writer
    writer.WriteLine("{")
    for field in recordFields do
        let fieldType = field.PropertyType

        if IsMultipleUnionCase fieldType then
            let cases = FSharpType.GetUnionCases(fieldType)
            cases
            |> Seq.iter(fun case ->
                WriteIndentationLevel 2 writer
                writer.WriteLine(sprintf "%s: %sDto list" case.Name (case.GetFields().[0].PropertyType.Name))
            )
        else
            WriteIndentationLevel 2 writer
            let (innerType, isOption, isList) = InnerType fieldType
            let typeModifier =
                if isOption then " option"
                else if isList then " list"
                else ""
            let typeName =
                if IsMorgemilType innerType && not (IsSingleCase innerType) && not (IsEnumUnion innerType) then
                    if IsRowKey innerType then
                        innerType.Name + "ID"
                    else
                        innerType.Name + "Dto"
                else innerType.Name
            writer.WriteLine(sprintf "%s: %s%s" field.Name typeName typeModifier)
    WriteIndentation writer
    writer.WriteLine("}")
    if IsRowKey currentType then
        WriteIndentation writer
        writer.WriteLine("interface IRow with")
        WriteIndentationLevel 2 writer
        writer.WriteLine("[<JsonIgnore>]")
        WriteIndentationLevel 2 writer
        writer.WriteLine("member this.Key = this.ID.Key")
    writer.WriteLine()



let TypesFromAssembly (assembly: Assembly) =
    let writer = new StringWriter()
    let typeSet = new System.Collections.Generic.List<string>()
    assembly.ExportedTypes
    |> Seq.filter (fun t -> t.GetCustomAttributes( typeof<Morgemil.Models.RecordSerializationAttribute>, true ).Length > 0)
    |> Seq.iter(fun t ->
        WriteDto writer t typeSet
        )
    writer.ToString()


[<EntryPoint>]
let main argv =
    let assembly = typeof<Morgemil.Models.Character>.Assembly


    File.WriteAllText("test.fs", TypesFromAssembly assembly)

    0 // return an integer exit code
