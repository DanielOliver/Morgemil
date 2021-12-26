module Morgemil.Generation.Creation

open System
open System.IO
open Morgemil.Generation.Analysis


//type SubstituedRecordType =
//    {
//        AstRecordType: AstRecordType
//    }
//and SubstitutedMultipleCaseUnion =
//    {
//        Cases: AstSingleCaseUnion list
//    }
//and SubstitutedCollectedType =
//    | MorgemilRecord of SubstituedRecordType
//    | MultipleCaseUnion of SubstitutedMultipleCaseUnion
//
//
//type [<RequireQualifiedAccess>] EndCollectedType =
//    | MorgemilRecord of AstRecordType
//    | MorgemilBase of Type
//    | Generic of AstGenericType
//    | SingleCaseUnion of AstSingleCaseUnion
//    | MultipleCaseUnion of AstSingleCaseUnion list
//    | EnumUnion of AstEnumUnion
//    | System of Type
//


type MappedRecordType =
    { ActualType: Type
      RecordIdField: string option
      Fields: Map<string, MappedRecordField> }

and MappedGenericType =
    { Type: MappedCollectedType
      WrappingTypes: KnownGenericType list }

and MappedRecordField =
    { FieldName: string
      Type: MappedCollectedType
      MeasureBy: string option }

and MappedSingleCaseUnion =
    { Type: MappedCollectedType
      CaseName: string
      UnionName: string }

and MappedMultipleCaseUnion =
    { Cases: MappedSingleCaseUnion list
      UnionName: string }

and MappedEnumUnion =
    { Cases: string list
      ActualType: Type }

and MappedType =
    | Unchanged of MappedCollectedType
    | Mapped of int

and [<RequireQualifiedAccess>] MappedCollectedType =
    | MorgemilRecord of MappedRecordType
    | MorgemilBase of Type
    | Generic of MappedGenericType
    | SingleCaseUnion of MappedSingleCaseUnion
    | MultipleCaseUnion of MappedMultipleCaseUnion
    | EnumUnion of MappedEnumUnion
    | System of Type



let rec describeType (t: AstCollectedType) : string =

    match t with
    | AstCollectedType.MorgemilRecord m ->
        if m.IsRowKey then
            "System.Int64"
        else
            m.ActualType.Name
    | AstCollectedType.MorgemilBase m -> m.Name + "Dto"
    | AstCollectedType.SingleCaseUnion m -> describeType m.Type
    | AstCollectedType.Generic m -> sprintf "%s %s" (describeType m.Type) (String.Join(" ", m.WrappingTypes))
    | AstCollectedType.System m -> m.FullName
    | AstCollectedType.MultipleCaseUnion m -> m.UnionName
    | AstCollectedType.EnumUnion m -> m.ActualType.Name



let indentationLevel = 4

let printType (writer: TextWriter) (record: AstRecordType) =
    let indent (level: int) =
        fprintf writer "%s" (String.replicate (indentationLevel * level) " ")

    writer.WriteLine()
    fprintfn writer "type %sDto =" record.ActualType.Name
    indent 1
    fprintfn writer "{"

    for (_, field) in record.Fields |> Map.toSeq do
        indent 2
        fprintf writer "%s: %s" field.FieldName (describeType field.Type)

        //        match field.Type with
//        | AstCollectedType.MorgemilRecord m ->
//            fprintf writer "%s" m.ActualType.Name
//        | AstCollectedType.MorgemilBase m ->
//            fprintf writer "%s" m.Name
//        | AstCollectedType.SingleCaseUnion m ->
//            fprintf writer "%s" m.Type
//        | _ -> ()


        fprintfn writer ""



    indent 1
    fprintfn writer "}"
