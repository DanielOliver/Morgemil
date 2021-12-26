module Morgemil.Generation.Analysis

open System
open System.Collections.Generic
open System.Reflection
open Microsoft.FSharp.Reflection

/// If is in a Morgemil namespace
let IsMorgemilType (t: Type) : bool = t.Namespace.StartsWith("Morgemil")

/// Exists with a Morgemil IRow Attribute, IE: is assignable via a numeric identifier
let HasMorgemilRowKeyAttribute (currentType: Type) =
    typeof<Morgemil.Models.Relational.IRow>.IsAssignableFrom (currentType)

let HasMorgemilRecordIdAttributeOnProperty (currentType: PropertyInfo) : bool =
    currentType.GetCustomAttributes(typeof<Morgemil.Models.RecordIdAttribute>)
    |> Seq.isEmpty
    |> not

let TryGetMorgemilMeasureByAttributeOnProperty (currentType: PropertyInfo) : Morgemil.Models.MeasureByAttribute option =
    match
        currentType.GetCustomAttributes(typeof<Morgemil.Models.MeasureByAttribute>)
        |> Seq.toList
        with
    | [ x ] -> x :?> Morgemil.Models.MeasureByAttribute |> Some
    | _ -> None

/// Exists with a Morgemil Record Attribute
let HasMorgemilRecordAttribute (t: Type) : bool =
    t
        .GetCustomAttributes(
            typeof<Morgemil.Models.RecordAttribute>,
            true
        )
        .Length > 0

/// If this type is a single case union, return the single contained field.
let TryGetUnionSingleCaseFieldType (t: Type) : PropertyInfo option =
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
let IsAnEnumUnionCase (t: Type) : bool =
    FSharpType.IsUnion(t)
    && FSharpType.GetUnionCases(t)
       |> Seq.forall
           (fun unionCase ->
               unionCase.GetFields().Length = 0
               && unionCase.Name
                  |> System.String.IsNullOrWhiteSpace
                  |> not)

/// If this type is a multiple case union where each case NO fields, return the case names
let TryGetUnionEnumCaseNames (t: Type) : string list option =
    match IsAnEnumUnionCase t with
    | true ->
        FSharpType.GetUnionCases(t)
        |> Array.map (fun u -> u.Name)
        |> Array.toList
        |> Some
    | false -> None

/// Returns true if this union has only field on every case, with more than one case.
let IsAnUnionWithMultipleCaseOfSingleField (t: Type) : bool =
    FSharpType.IsUnion(t)
    && FSharpType.GetUnionCases(t)
       |> Seq.forall
           (fun unionCase ->
               unionCase.GetFields().Length = 1
               && unionCase.Name
                  |> System.String.IsNullOrWhiteSpace
                  |> not)
    && FSharpType.GetUnionCases(t).Length > 1

type SingleFieldCaseInfo =
    { Case: UnionCaseInfo
      Property: PropertyInfo }

/// If this type is a multiple case union where each case only has one field each, return the cases.
let TryGetUnionMultipleCases (t: Type) : SingleFieldCaseInfo list option =
    match IsAnUnionWithMultipleCaseOfSingleField t with
    | true ->
        FSharpType.GetUnionCases(t)
        |> Array.map
            (fun u ->
                let field = u.GetFields().[0]

                { SingleFieldCaseInfo.Case = u
                  Property = field })
        |> Array.toList
        |> Some
    | false -> None

[<RequireQualifiedAccess>]
type KnownGenericType =
    | List
    | Option

type GenericTypeDeconstruct =
    { InnerType: Type
      WrappingGenericTypes: KnownGenericType list }
/// If a generic type, decomposes this type into the generic hierarchy.
/// The first item on the list is the inner-most wrapping generic type.
let TryGetInnerTypes (t: Type) : GenericTypeDeconstruct option =
    let rec recurse (t: Type) (generics: KnownGenericType list) : (Type * KnownGenericType list) =
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
        { GenericTypeDeconstruct.InnerType = innerType
          WrappingGenericTypes = wrappers }
        |> Some

/// Gets all fields from a record
let GetRecordFields (t: Type) : PropertyInfo array =
    if FSharpType.IsRecord t then
        FSharpType.GetRecordFields t
    else
        [||]

let (|MatchInnerTypes|_|) (t: Type) : GenericTypeDeconstruct option = TryGetInnerTypes t
let (|MatchUnionSingleCase|_|) (t: Type) : PropertyInfo option = TryGetUnionSingleCaseFieldType t
let (|MatchUnionMultipleCase|_|) (t: Type) : SingleFieldCaseInfo list option = TryGetUnionMultipleCases t
let (|MatchUnionEnumCaseNames|_|) (t: Type) : string list option = TryGetUnionEnumCaseNames t


type AstRecordType =
    { ActualType: Type
      RecordIdField: string option
      Fields: Map<string, AstRecordField> }
    member this.IsRecord =
        HasMorgemilRecordAttribute this.ActualType

    member this.IsRowKey =
        HasMorgemilRowKeyAttribute this.ActualType

and AstGenericType =
    { Type: AstCollectedType
      WrappingTypes: KnownGenericType list }

and AstRecordField =
    { FieldName: string
      Type: AstCollectedType
      MeasureBy: string option }

and AstSingleCaseUnion =
    { Type: AstCollectedType
      CaseName: string
      UnionName: string
      ActualType: Type }

and AstMultipleCaseUnion =
    { Cases: AstSingleCaseUnion list
      UnionName: string
      ActualType: Type }

and AstEnumUnion =
    { Cases: string list
      ActualType: Type }

and [<RequireQualifiedAccess>] AstCollectedType =
    | MorgemilRecord of AstRecordType
    | MorgemilBase of Type
    | Generic of AstGenericType
    | SingleCaseUnion of AstSingleCaseUnion
    | MultipleCaseUnion of AstMultipleCaseUnion
    | EnumUnion of AstEnumUnion
    | System of Type

let rec AnalyzeType (t: Type) : AstCollectedType =
    if (t.IsGenericType) then
        match TryGetInnerTypes t with
        | None -> failwithf "FAILURE TO GET GENERIC TYPE?!?!?!?! of %A" t
        | Some deconstruction ->
            AstCollectedType.Generic
                { Type = (AnalyzeType deconstruction.InnerType)
                  WrappingTypes = deconstruction.WrappingGenericTypes }
    else

    if not (IsMorgemilType t) then
        AstCollectedType.System t
    else

    if FSharpType.IsUnion t then
        match t with
        | MatchUnionEnumCaseNames (caseNames) ->
            AstCollectedType.EnumUnion
                { AstEnumUnion.Cases = caseNames
                  ActualType = t }
        | MatchUnionSingleCase (case) ->
            AstCollectedType.SingleCaseUnion
                { AstSingleCaseUnion.CaseName = case.Name
                  Type = AnalyzeType case.PropertyType
                  UnionName = t.Name
                  ActualType = t }
        | MatchUnionMultipleCase (cases) ->
            { AstMultipleCaseUnion.Cases =
                  cases
                  |> List.map
                      (fun case ->
                          { AstSingleCaseUnion.CaseName = case.Case.Name
                            Type = AnalyzeType case.Property.PropertyType
                            UnionName = t.Name
                            ActualType = t })
              UnionName = t.Name
              ActualType = t }
            |> AstCollectedType.MultipleCaseUnion

        | _ -> failwithf "UNION FAILURE! -> %A" t
    else

    if not (FSharpType.IsRecord t) then
        AstCollectedType.MorgemilBase t
    else

        let mutable recordKeyId = None

        let fields =
            GetRecordFields t
            |> Seq.map
                (fun field ->
                    if HasMorgemilRecordIdAttributeOnProperty field then
                        recordKeyId <- Some field.Name

                    (field.Name,
                     { AstRecordField.Type = field.PropertyType |> AnalyzeType
                       FieldName = field.Name
                       MeasureBy =
                           field
                           |> TryGetMorgemilMeasureByAttributeOnProperty
                           |> Option.map (fun i -> i.Name) }))
            |> Map.ofSeq

        AstCollectedType.MorgemilRecord
            { AstRecordType.ActualType = t
              Fields = fields
              RecordIdField = recordKeyId }

type DependencyGraph =
    { Type: Type
      DependentTypes: Dictionary<string, Type> }
    member this.TypeFullName = this.Type.FullName

let ReadDependencyGraph (t: AstCollectedType) (history: Dictionary<string, DependencyGraph>) : unit =
    let addType (foundType: Type) (parentType: Type) : unit =
        let fullname = parentType.FullName

        match history.TryGetValue fullname with
        | true, value ->
            value.DependentTypes.TryAdd(foundType.FullName, foundType)
            |> ignore
        | _ ->
            let dependentTypes = new Dictionary<string, Type>()
            dependentTypes.Add(foundType.FullName, foundType)

            history.Add(
                fullname,
                { DependencyGraph.Type = parentType
                  DependentTypes = dependentTypes }
            )

    //    let shouldParseType (t: Type) = not(history.ContainsKey(t.FullName))

    let rec descend (t: AstCollectedType) : unit =
        match t with
        | AstCollectedType.MorgemilRecord m ->
            m.Fields
            |> Map.toSeq
            |> Seq.iter (fun (_, f) -> descend2 f.Type m.ActualType)
        | _ -> failwithf "FAILURE TO UNDERSTAND! %A" t

    and descend2 (t: AstCollectedType) (parent: Type) : unit =
        match t with
        | AstCollectedType.MorgemilRecord m ->
            addType m.ActualType parent

            m.Fields
            |> Map.toSeq
            |> Seq.iter (fun (_, f) -> descend2 f.Type m.ActualType)
        | AstCollectedType.MorgemilBase m -> addType m parent
        | AstCollectedType.Generic m -> descend2 m.Type parent
        | AstCollectedType.SingleCaseUnion m ->
            addType m.ActualType parent
            descend2 m.Type parent
        | AstCollectedType.MultipleCaseUnion m ->
            m.Cases
            |> Seq.iter (fun f -> descend2 f.Type m.ActualType)

            addType m.ActualType parent
        | AstCollectedType.EnumUnion m -> addType m.ActualType parent
        | AstCollectedType.System m -> addType m parent

    descend t
