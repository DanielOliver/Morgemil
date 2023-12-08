module Morgemil.Core.Tests.Relational

open Morgemil.Models
open Xunit
open Morgemil.Core
open Morgemil.Models.Relational

// [<Struct>]
// type CharacterID = CharacterID of int64

type ExampleRow =
    { Name: string
      Attribute1: int
      CharacterID: CharacterID }

    interface IRow with
        member this.Key =
            let (CharacterID key) = this.CharacterID
            key

type ExampleTable() as this =
    inherit
        Table<CharacterAttributes, CharacterID>(
            CharacterID,
            (fun (CharacterID value) -> value),
            StepItem.CharacterAttributes
        )

    let _multiIndexByName = new MultiIndex<CharacterAttributes, string>(_.Ancestry.Noun)

    do this.AddIndex(_multiIndexByName)

    member this.NameIndex = _multiIndexByName


[<Fact>]
let ``Can Add items to Index`` () =
    let exampleTable = new ExampleTable()

    Assert.Equal(CharacterID 0L, Table.GenerateKey exampleTable)

    let exampleItem1 =
        { CharacterAttributes.ID = CharacterID(500L)
          Ancestry =
            { Adjective = "asdf"
              Description = "324"
              Noun = "Test1"
              Tags = Map.empty
              ID = AncestryID(5L)
              RequireTags = Map.empty }
          Heritage = []
          Tags = Map.empty }

    Table.AddRow exampleTable exampleItem1

    let exampleItem2 =
        { CharacterAttributes.ID = CharacterID(501L)
          Ancestry =
            { Adjective = "asdf"
              Description = "324"
              Noun = "Test1"
              Tags = Map.empty
              ID = AncestryID(5L)
              RequireTags = Map.empty }
          Heritage = []
          Tags = Map.empty }

    Table.AddRow exampleTable exampleItem2

    Assert.Equal(CharacterID 502L, Table.GenerateKey exampleTable)
    Assert.Equal(2, exampleTable |> Table.Items |> Seq.length)

    Assert.Throws<System.Exception>(fun () -> Table.GetRowByKey exampleTable (CharacterID 123L) |> ignore)
    |> ignore

    Assert.Equal(exampleItem1, Table.GetRowByKey exampleTable (CharacterID 500L))

    Assert.Equal(2, (MultiIndex.GetRowsByKey exampleTable.NameIndex "Test1") |> Seq.length)

    Assert.Empty(MultiIndex.GetRowsByKey exampleTable.NameIndex "Test1234234234")

    Table.RemoveRow exampleTable exampleItem1
    Assert.Equal(1, exampleTable |> Table.Items |> Seq.length)

    Assert.Throws<System.Exception>(fun () -> Table.GetRowByKey exampleTable (CharacterID 500L) |> ignore)
    |> ignore

    Assert.Equal(1, (MultiIndex.GetRowsByKey exampleTable.NameIndex "Test1") |> Seq.length)

    let exampleItem3 =
        { exampleItem2 with
            Ancestry =
                { exampleItem2.Ancestry with
                    Noun = "onetwo" } }

    Table.AddRow exampleTable exampleItem3
    Assert.Equal(exampleItem3, Table.GetRowByKey exampleTable (CharacterID 501L))
    Assert.Empty(MultiIndex.GetRowsByKey exampleTable.NameIndex "Test1")

    Assert.Equal(1, (MultiIndex.GetRowsByKey exampleTable.NameIndex "onetwo") |> Seq.length)

    for index in [ 1L .. 50_000L ] do
        { exampleItem2 with
            ID = CharacterID(index)
            Ancestry =
                { exampleItem2.Ancestry with
                    Noun = index.ToString() } }
        |> Table.AddRow exampleTable

    Assert.Equal(50_000, exampleTable |> Table.Items |> Seq.length)

    for item in exampleTable |> Table.Items |> Seq.toList do
        { item with
            Ancestry = { item.Ancestry with Noun = "asdf" } }
        |> Table.AddRow exampleTable

    Assert.Equal(50_000, exampleTable |> Table.Items |> Seq.length)

    for index in [ 1L .. 10_000L ] do
        index |> CharacterID |> Table.RemoveRowByKey exampleTable

    Assert.Equal(40_000, exampleTable |> Table.Items |> Seq.length)

    for index in [ 1L .. 50_000L ] do
        index |> CharacterID |> Table.RemoveRowByKey exampleTable

    Assert.Equal(0, exampleTable |> Table.Items |> Seq.length)

    let readOnlyTable =
        Table.CreateReadonlyTable (fun (CharacterID value) -> value) [ exampleItem1; exampleItem2 ]

    Assert.Equal(2, readOnlyTable.Items |> Seq.length)

    Assert.Equal(exampleItem1, 500L |> CharacterID |> Table.GetRowByKey readOnlyTable)

    Assert.Equal(exampleItem2, 501L |> CharacterID |> Table.GetRowByKey readOnlyTable)



type ExampleRowNext =
    { Name: string
      CharacterIDID: int64
      ForeignKeyID: int64 }

    interface IRow with
        member this.Key = this.CharacterIDID


type ExampleRowNext234 =
    { Name: string
      CharacterIDID: int64
      ForeignKeyID: int64 }

    interface IRow with
        member this.Key = this.CharacterIDID

[<Fact>]
let ``Check Joins on readonlyArrays`` () =

    let exampleItem1 =
        { ExampleRow.Attribute1 = 0
          CharacterID = CharacterID(500L)
          Name = "Test1" }

    let exampleItem2 =
        { ExampleRow.Attribute1 = 23
          CharacterID = CharacterID(501L)
          Name = "Test1" }

    let exampleItem3 = { exampleItem2 with Name = "onetwo" }

    let row1 =
        { ExampleRowNext.Name = "one"
          CharacterIDID = 234L
          ForeignKeyID = 500L }

    let row2 =
        { ExampleRowNext.Name = "two"
          CharacterIDID = 235L
          ForeignKeyID = 501L }

    let row3 =
        { ExampleRowNext.Name = "two"
          CharacterIDID = 235L
          ForeignKeyID = 0L }

    let exampleItem234 =
        { ExampleRowNext234.Name = "two"
          CharacterIDID = 235L
          ForeignKeyID = 0L }

    let tableOne =
        Table.CreateReadonlyTable (fun (CharacterID value) -> value) [ exampleItem1; exampleItem2; exampleItem3 ]

    let tableTwo =
        Table.CreateReadonlyTable (fun (CharacterID value) -> value) [ row1; row2; row3 ]

    let tableThree =
        Table.CreateReadonlyTable (fun (CharacterID value) -> value) [ exampleItem234 ]

    let joinedRows =
        TableQuery.LeftJoin tableTwo (fun t -> t.ForeignKeyID |> CharacterID) tableOne
        |> Seq.toArray

    Assert.Equal(3, joinedRows.Length)

    let row, exampleItem = joinedRows[2]
    Assert.True(exampleItem.IsNone)
    Assert.Equal(row3, row)

    let row, exampleItem = joinedRows[1]
    Assert.Equal(exampleItem2, exampleItem.Value)
    Assert.Equal(row2, row)
