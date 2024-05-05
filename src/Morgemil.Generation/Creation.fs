module Morgemil.Generation.Creation

open System
open Fantomas.Core
open Fantomas.FCS.Text
open Fantomas.Core.SyntaxOak
open Morgemil.Generation.Analysis


let oakSingleText text = SingleTextNode(text, Range.Zero)

type RecordToDtoParamters =
    { Namespace: string list }

    static member Default = { RecordToDtoParamters.Namespace = [ "Morgemil"; "Dto" ] }

let translateRecordToDto (ast: AstCollectedType) (config: RecordToDtoParamters) : string =
    match ast with
    | AstCollectedType.MorgemilRecord record ->

        let defnRecordFields =
            record.Fields
            |> Seq.map (fun field ->
                FieldNode(
                    None,
                    None,
                    None,
                    None,
                    None,
                    Some(oakSingleText field.Key),
                    Fantomas.Core.SyntaxOak.Type.Anon(oakSingleText field.Value.Type.UnderlyingType.FullName),
                    Range.Zero
                ))
            |> Seq.toList

        let defnRecord =
            TypeDefnRecordNode(
                TypeNameNode(
                    None,
                    None,
                    oakSingleText "type",
                    None,
                    IdentListNode([ IdentifierOrDot.Ident(oakSingleText record.ActualType.Name) ], Range.Zero),
                    None,
                    [],
                    None,
                    None,
                    None,
                    Range.Zero
                ),
                None,
                oakSingleText "{",
                defnRecordFields,
                oakSingleText "}",
                [],
                Range.Zero
            )

        Oak(
            [],
            [ ModuleOrNamespaceNode(
                  Some(
                      ModuleOrNamespaceHeaderNode(
                          None,
                          None,
                          MultipleTextsNode([ oakSingleText "namespace" ], Range.Zero),
                          None,
                          false,
                          Some(
                              IdentListNode(
                                  config.Namespace
                                  |> List.collect (fun nameSpace ->
                                      [ IdentifierOrDot.KnownDot(oakSingleText ".")
                                        IdentifierOrDot.Ident(oakSingleText nameSpace) ])
                                  |> List.skip 1,
                                  Range.Zero
                              )
                          ),
                          Range.Zero
                      )
                  ),
                  [ ModuleDecl.TypeDefn(TypeDefn.Record defnRecord) ],
                  Range.Zero
              ) ],
            Range.Zero
        )
        |> CodeFormatter.FormatOakAsync
        |> Async.RunSynchronously
    | _ -> String.Empty
