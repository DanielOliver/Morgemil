module Morgemil.Core.Tests.Relational

open Xunit
open Morgemil.Core

[<Struct>]
type ExampleKey = 
    | ExampleKey of int64

type ExampleRow =
    {   Name: string
        Attribute1: int
        ExampleKey: ExampleKey
    }

    interface IRow with
        member this.Key =
            let (ExampleKey key) = this.ExampleKey
            key

type ExampleTable() as this =
    inherit Table<ExampleRow>()

    let _multiIndexByName = new MultiIndex<ExampleRow, string>(fun x -> x.Name)
    do
        this.AddIndex(_multiIndexByName)

    member this.NameIndex = _multiIndexByName


    

[<Fact>]
let ``Can Add items to Index``() =
    let exampleTable = new ExampleTable()

    Assert.Equal(0L, Table.GenerateKey exampleTable)

    let exampleItem1 = {
        ExampleRow.Attribute1 = 0
        ExampleKey = ExampleKey(500L)
        Name = "Test1"
    }
    Table.AddRow exampleTable exampleItem1
    
    let exampleItem2 = {
        ExampleRow.Attribute1 = 23
        ExampleKey = ExampleKey(501L)
        Name = "Test1"
    }
    Table.AddRow exampleTable exampleItem2
    
    Assert.Equal(502L, Table.GenerateKey exampleTable)
    Assert.Equal(2, exampleTable |> Table.Items |> Seq.length)
    Assert.Throws<System.Exception>(fun () -> Table.GetRowByKey exampleTable 123L |> ignore) |> ignore
    Assert.Equal(exampleItem1, Table.GetRowByKey exampleTable 500L)    
    Assert.Equal(2, (MultiIndex.GetRowsByKey exampleTable.NameIndex "Test1").Length)
    Assert.Empty(MultiIndex.GetRowsByKey exampleTable.NameIndex "Test1234234234")
    
    Table.RemoveRow exampleTable exampleItem1
    Assert.Equal(1, exampleTable |> Table.Items |> Seq.length)
    Assert.Throws<System.Exception>(fun () -> Table.GetRowByKey exampleTable 500L |> ignore) |> ignore
    Assert.Equal(1, (MultiIndex.GetRowsByKey exampleTable.NameIndex "Test1").Length)
    
    let exampleItem3 = {
        exampleItem2 with
            Name = "onetwo"
    }
    Table.AddRow exampleTable exampleItem3
    Assert.Equal(exampleItem3, Table.GetRowByKey exampleTable 501L)
    Assert.Empty(MultiIndex.GetRowsByKey exampleTable.NameIndex "Test1")
    Assert.Equal(1, (MultiIndex.GetRowsByKey exampleTable.NameIndex "onetwo").Length)

    for index in [1L..50_000L] do
        {
            ExampleRow.Attribute1 = int(index) * 2
            ExampleKey = ExampleKey(index)
            Name = "Index " + index.ToString()
        }
        |> Table.AddRow exampleTable
        
    Assert.Equal(50_000, exampleTable |> Table.Items |> Seq.length)

    for item in exampleTable |> Table.Items |> Seq.toList do
        {   item with
                Name = item.Name + " Name"
        }
        |> Table.AddRow exampleTable
        
    Assert.Equal(50_000, exampleTable |> Table.Items |> Seq.length)
    
    for index in [1L..10_000L] do
        index
        |> Table.RemoveRowByKey exampleTable
        
    Assert.Equal(40_000, exampleTable |> Table.Items |> Seq.length)

    for index in [1L..50_000L] do
        index
        |> Table.RemoveRowByKey exampleTable
        
    Assert.Equal(0, exampleTable |> Table.Items |> Seq.length)

