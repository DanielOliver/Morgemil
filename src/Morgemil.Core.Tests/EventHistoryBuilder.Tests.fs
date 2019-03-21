module Morgemil.Core.Tests.EventHistoryBuilder

open Xunit
open Morgemil.Core
open Morgemil.Models
open Morgemil.Math
open Morgemil.Models.Relational


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
    inherit Table<ExampleRow, ExampleKey>(ExampleKey, (fun (ExampleKey value) -> value))

    let _multiIndexByName = new MultiIndex<ExampleRow, string>(fun x -> x.Name)
    do
        this.AddIndex(_multiIndexByName)

    member this.NameIndex = _multiIndexByName

let exampleItem1 =
    ActionEvent.AfterMove <|
        {
            CharacterID = CharacterID 50L
            OldPosition = Vector2i.Zero
            NewPosition = Vector2i.Zero
        }
let exampleItem2 =
    ActionEvent.AfterMove <|
        {
            CharacterID = CharacterID 51L
            OldPosition = Vector2i.Identity
            NewPosition = Vector2i.Identity
        }
let exampleItem3 =
    ActionEvent.AfterMove <|
        {
            CharacterID = CharacterID 52L
            OldPosition = Vector2i.Identity
            NewPosition = Vector2i.Zero
        }

[<Fact>]
let ``Can yield Results without updates``() =
    let table1 = ExampleTable()
    let eventBuilder = EventHistoryBuilder table1
    let results =
        eventBuilder {
            yield exampleItem1
            yield eventBuilder {
                yield exampleItem3
                yield exampleItem1
            }
            yield exampleItem2
        }
    Assert.Equal(4, results.Length)
    Assert.Equal< ExampleRow Step list>(
        [ exampleItem2; exampleItem1; exampleItem3; exampleItem1 ] |> List.map(fun t -> { Step.Event = t; Updates = [] }),
        results)

        
[<Fact>]
let ``Can yield Results with updates``() =
    let exampleTable = ExampleTable()
    
    let exampleRow1 = {
        ExampleRow.Attribute1 = 0
        ExampleKey = ExampleKey(500L)
        Name = "Test1"
    }
    let exampleRow2 = {
        ExampleRow.Attribute1 = 23
        ExampleKey = ExampleKey(501L)
        Name = "Test1"
    }

    let eventBuilder = EventHistoryBuilder exampleTable

    let results =
        eventBuilder {
            Table.AddRow exampleTable exampleRow1
            yield exampleItem1
            Table.RemoveRow exampleTable exampleRow1
            Table.AddRow exampleTable exampleRow2
            yield exampleItem2
        }
    Assert.Equal< ExampleRow Step list>(
        [
            {
                Step.Event = exampleItem2
                Updates = [
                    TableEvent.Added exampleRow2
                    TableEvent.Removed exampleRow1
                ]
            }
            {
                Step.Event = exampleItem1
                Updates = [ TableEvent.Added exampleRow1 ]
            }
        ],
        results)

