module Morgemil.Core.Tests.Relational

open Xunit
open Morgemil.Core
open Morgemil.Models.Relational

[<Struct>]
type ExampleKey = ExampleKey of int64

type ExampleRow =
    { Name: string
      Attribute1: int
      ExampleKey: ExampleKey }

    interface IRow with
        member this.Key =
            let (ExampleKey key) = this.ExampleKey
            key

type ExampleTable() as this =
    inherit Table<ExampleRow, ExampleKey>(ExampleKey, (fun (ExampleKey value) -> value))

    let _multiIndexByName =
        new MultiIndex<ExampleRow, string>(fun x -> x.Name)

    do this.AddIndex(_multiIndexByName)

    member this.NameIndex = _multiIndexByName




[<Fact>]
let ``Can Add items to Index`` () =
    let exampleTable = new ExampleTable()

    Assert.Equal(ExampleKey 0L, Table.GenerateKey exampleTable)

    let exampleItem1 =
        { ExampleRow.Attribute1 = 0
          ExampleKey = ExampleKey(500L)
          Name = "Test1" }

    Table.AddRow exampleTable exampleItem1

    let exampleItem2 =
        { ExampleRow.Attribute1 = 23
          ExampleKey = ExampleKey(501L)
          Name = "Test1" }

    Table.AddRow exampleTable exampleItem2

    Assert.Equal(ExampleKey 502L, Table.GenerateKey exampleTable)
    Assert.Equal(2, exampleTable |> Table.Items |> Seq.length)

    Assert.Throws<System.Exception>
        (fun () ->
            Table.GetRowByKey exampleTable (ExampleKey 123L)
            |> ignore)
    |> ignore

    Assert.Equal(exampleItem1, Table.GetRowByKey exampleTable (ExampleKey 500L))

    Assert.Equal(
        2,
        (MultiIndex.GetRowsByKey exampleTable.NameIndex "Test1")
        |> Seq.length
    )

    Assert.Empty(MultiIndex.GetRowsByKey exampleTable.NameIndex "Test1234234234")

    Table.RemoveRow exampleTable exampleItem1
    Assert.Equal(1, exampleTable |> Table.Items |> Seq.length)

    Assert.Throws<System.Exception>
        (fun () ->
            Table.GetRowByKey exampleTable (ExampleKey 500L)
            |> ignore)
    |> ignore

    Assert.Equal(
        1,
        (MultiIndex.GetRowsByKey exampleTable.NameIndex "Test1")
        |> Seq.length
    )

    let exampleItem3 = { exampleItem2 with Name = "onetwo" }
    Table.AddRow exampleTable exampleItem3
    Assert.Equal(exampleItem3, Table.GetRowByKey exampleTable (ExampleKey 501L))
    Assert.Empty(MultiIndex.GetRowsByKey exampleTable.NameIndex "Test1")

    Assert.Equal(
        1,
        (MultiIndex.GetRowsByKey exampleTable.NameIndex "onetwo")
        |> Seq.length
    )

    for index in [ 1L .. 50_000L ] do
        { ExampleRow.Attribute1 = int (index) * 2
          ExampleKey = ExampleKey(index)
          Name = "Index " + index.ToString() }
        |> Table.AddRow exampleTable

    Assert.Equal(50_000, exampleTable |> Table.Items |> Seq.length)

    for item in exampleTable |> Table.Items |> Seq.toList do
        { item with Name = item.Name + " Name" }
        |> Table.AddRow exampleTable

    Assert.Equal(50_000, exampleTable |> Table.Items |> Seq.length)

    for index in [ 1L .. 10_000L ] do
        index
        |> ExampleKey
        |> Table.RemoveRowByKey exampleTable

    Assert.Equal(40_000, exampleTable |> Table.Items |> Seq.length)

    for index in [ 1L .. 50_000L ] do
        index
        |> ExampleKey
        |> Table.RemoveRowByKey exampleTable

    Assert.Equal(0, exampleTable |> Table.Items |> Seq.length)

    let readOnlyTable =
        Table.CreateReadonlyTable(fun (ExampleKey value) -> value) [ exampleItem1; exampleItem2 ]

    Assert.Equal(2, readOnlyTable.Items |> Seq.length)

    Assert.Equal(
        exampleItem1,
        500L
        |> ExampleKey
        |> Table.GetRowByKey readOnlyTable
    )

    Assert.Equal(
        exampleItem2,
        501L
        |> ExampleKey
        |> Table.GetRowByKey readOnlyTable
    )



type ExampleRowNext =
    { Name: string
      ExampleKeyID: int64
      ForeignKeyID: int64 }
    interface IRow with
        member this.Key = this.ExampleKeyID


type ExampleRowNext234 =
    { Name: string
      ExampleKeyID: int64
      ForeignKeyID: int64 }
    interface IRow with
        member this.Key = this.ExampleKeyID

[<Fact>]
let ``Check Joins on readonlyArrays`` () =

    let exampleItem1 =
        { ExampleRow.Attribute1 = 0
          ExampleKey = ExampleKey(500L)
          Name = "Test1" }

    let exampleItem2 =
        { ExampleRow.Attribute1 = 23
          ExampleKey = ExampleKey(501L)
          Name = "Test1" }

    let exampleItem3 = { exampleItem2 with Name = "onetwo" }

    let row1 =
        { ExampleRowNext.Name = "one"
          ExampleKeyID = 234L
          ForeignKeyID = 500L }

    let row2 =
        { ExampleRowNext.Name = "two"
          ExampleKeyID = 235L
          ForeignKeyID = 501L }

    let row3 =
        { ExampleRowNext.Name = "two"
          ExampleKeyID = 235L
          ForeignKeyID = 0L }

    let exampleItem234 =
        { ExampleRowNext234.Name = "two"
          ExampleKeyID = 235L
          ForeignKeyID = 0L }

    let tableOne =
        Table.CreateReadonlyTable
            (fun (ExampleKey value) -> value)
            [ exampleItem1
              exampleItem2
              exampleItem3 ]

    let tableTwo =
        Table.CreateReadonlyTable(fun (ExampleKey value) -> value) [ row1; row2; row3 ]

    let tableThree =
        Table.CreateReadonlyTable(fun (ExampleKey value) -> value) [ exampleItem234 ]

    let joinedRows =
        TableQuery.LeftJoin tableTwo (fun t -> t.ForeignKeyID |> ExampleKey) tableOne
        |> Seq.toArray

    Assert.Equal(3, joinedRows.Length)

    let (row, exampleItem) = joinedRows.[2]
    Assert.True(exampleItem.IsNone)
    Assert.Equal(row3, row)

    let (row, exampleItem) = joinedRows.[1]
    Assert.Equal(exampleItem2, exampleItem.Value)
    Assert.Equal(row2, row)
