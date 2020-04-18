module Morgemil.Generation.Creation

open System
open System.Reflection
open Microsoft.FSharp.Reflection

/// If is in a Morgemil namespace
let IsMorgemilType(t: Type): bool =
    t.Namespace.StartsWith("Morgemil")

/// Exists with a Morgemil IRow Attribute, IE: is assignable via a numeric identifier
let HasMorgemilRowKeyAttribute (currentType: Type) =
    typeof<Morgemil.Models.Relational.IRow>.IsAssignableFrom(currentType)

let HasMorgemilRecordIdAttributeOnProperty (currentType: PropertyInfo): bool =
    currentType.GetCustomAttributes(typeof<Morgemil.Models.RecordIdAttribute>) |> Seq.isEmpty |> not

/// Exists with a Morgemil Record Attribute
let HasMorgemilRecordAttribute (t: Type): bool =
    t.GetCustomAttributes( typeof<Morgemil.Models.RecordAttribute>, true ).Length > 0

/// If this type is a single case union, return the single contained field.
let TryGetUnionSingleCaseFieldType(t: Type): PropertyInfo option =
    match FSharpType.IsUnion(t) with
    | true ->
        match FSharpType.GetUnionCases(t) with
        | [| case |] ->
            match case.GetFields() with
            | [| field |] -> Some field
            | _ -> None
        | _ -> None
    | false -> None

/// Returns true if this union has no fields on any case.
let IsAnEnumUnionCase(t: Type): bool =
    FSharpType.IsUnion(t)
    && FSharpType.GetUnionCases(t)
        |> Seq.forall(fun unionCase ->
            unionCase.GetFields().Length = 0
            && unionCase.Name |> System.String.IsNullOrWhiteSpace |> not
        )

/// If this type is a multiple case union where each case NO fields, return the case names
let TryGetUnionEnumCaseNames(t: Type): string list option =
    match IsAnEnumUnionCase t with
    | true ->
        FSharpType.GetUnionCases(t)
        |> Array.map (fun u -> u.Name)
        |> Array.toList
        |> Some
    | false -> None

/// Returns true if this union has only field on every case, with more than one case.
let IsAnUnionWithMultipleCaseOfSingleField(t: Type): bool =
    FSharpType.IsUnion(t)
    && FSharpType.GetUnionCases(t)
        |> Seq.forall(fun unionCase ->
            unionCase.GetFields().Length = 1
            && unionCase.Name |> System.String.IsNullOrWhiteSpace |> not
        )
    && FSharpType.GetUnionCases(t).Length > 1

type SingleFieldCaseInfo = {
    Case: UnionCaseInfo
    Property: PropertyInfo
}

/// If this type is a multiple case union where each case only has one field each, return the cases.
let TryGetUnionMultipleCases(t: Type): SingleFieldCaseInfo list option =
    match IsAnUnionWithMultipleCaseOfSingleField t with
    | true ->
        FSharpType.GetUnionCases(t)
        |> Array.map (fun u ->
            let field = u.GetFields().[0]
            {
                SingleFieldCaseInfo.Case = u
                Property = field
            })
        |> Array.toList
        |> Some
    | false -> None

[<RequireQualifiedAccess>]
type KnownGenericType =
    | List
    | Option
type GenericTypeDeconstruct = {
    InnerType: Type
    WrappingGenericTypes: KnownGenericType list
}
/// If a generic type, decomposes this type into the generic hierarchy.
/// The first item on the list is the inner-most wrapping generic type.
let TryGetInnerTypes (t: Type): GenericTypeDeconstruct option =
    let rec recurse (t: Type) (generics: KnownGenericType list): (Type * KnownGenericType list) =
        if t.IsGenericType then
            let arg0 = t.GenericTypeArguments.[0]
            let genType = t.GetGenericTypeDefinition()
            if genType = typedefof<option<_>> then
                recurse arg0 (KnownGenericType.Option :: generics)
            else if genType = typedefof<list<_>> then
                recurse arg0 (KnownGenericType.List :: generics)
            else
                failwithf "What is generic type %s" (t.GetGenericTypeDefinition().Name)
        else
            (t, generics)
    match recurse t [] with
    | _, [] -> None
    | (innerType, wrappers) ->
        {
            GenericTypeDeconstruct.InnerType = innerType
            WrappingGenericTypes = wrappers
        } |> Some

/// Gets all fields from a record
let GetRecordFields (t: Type): PropertyInfo array =
    if FSharpType.IsRecord t then
        FSharpType.GetRecordFields t
    else [||]

let (|MatchInnerTypes|_|) (t: Type): GenericTypeDeconstruct option = TryGetInnerTypes t
let (|MatchUnionSingleCase|_|) (t: Type): PropertyInfo option = TryGetUnionSingleCaseFieldType t
let (|MatchUnionMultipleCase|_|) (t: Type): SingleFieldCaseInfo list option = TryGetUnionMultipleCases t
let (|MatchUnionEnumCaseNames|_|) (t: Type): string list option = TryGetUnionEnumCaseNames t


type RecordType =
    {
        Type: Type
        RecordIdField: string option
        Fields: Map<string, CollectedType>
    }
    member this.IsRecord = HasMorgemilRecordAttribute this.Type
    member this.IsRowKey = HasMorgemilRowKeyAttribute this.Type
and GenericType =
    {
        Type: CollectedType
        WrappingTypes: KnownGenericType list
    }
and SingleCaseUnion =
    {
        Type: CollectedType
        CaseName: string
    }
and EnumUnion =
    {
        Cases: string list
        Type: Type
    }
and [<RequireQualifiedAccess>] CollectedType =
    | MorgemilRecord of RecordType
    | MorgemilBase of Type
    | Generic of GenericType
    | SingleCaseUnion of SingleCaseUnion
    | MultipleCaseUnion of SingleCaseUnion list
    | EnumUnion of EnumUnion
    | System of Type

let rec AnalyzeType (t: Type): CollectedType =
    if (t.IsGenericType) then
        match TryGetInnerTypes t with
        | None -> failwithf "FAILURE TO GET GENERIC TYPE?!?!?!?! of %A" t
        | Some deconstruction ->
            CollectedType.Generic {
                Type = (AnalyzeType deconstruction.InnerType)
                WrappingTypes = deconstruction.WrappingGenericTypes
            }
    else

    if not(IsMorgemilType t) then
        CollectedType.System t
    else

    if FSharpType.IsUnion t then
        match t with
        | MatchUnionEnumCaseNames(caseNames) ->
            CollectedType.EnumUnion {
                EnumUnion.Cases = caseNames
                Type = t
            }
        | MatchUnionSingleCase(case) ->
            CollectedType.SingleCaseUnion {
                SingleCaseUnion.CaseName = case.Name
                Type = AnalyzeType case.PropertyType
            }
        | MatchUnionMultipleCase(cases) ->
            cases
            |> List.map (fun case -> {
                SingleCaseUnion.CaseName = case.Case.Name
                Type = AnalyzeType case.Property.PropertyType
            })
            |> CollectedType.MultipleCaseUnion

        | _ -> failwithf "UNION FAILURE! -> %A" t
    else

    if not(FSharpType.IsRecord t) then
        CollectedType.MorgemilBase t
    else

    let mutable recordKeyId = None
    let fields =
        GetRecordFields t
        |> Seq.map(fun field ->
            if HasMorgemilRecordIdAttributeOnProperty field then
                recordKeyId <- Some field.Name

            (field.Name, field.PropertyType |> AnalyzeType)
        )
        |> Map.ofSeq
    CollectedType.MorgemilRecord {
        RecordType.Type = t
        Fields = fields
        RecordIdField = recordKeyId
    }

