module Morgemil.Core.Tests.Relational

open Morgemil.Models
open Morgemil.Core
open Morgemil.Models.Relational
open Xunit

// [<Struct>]
// type CharacterID = CharacterID of int64

type ExampleRow =
    { Name: string
      Attribute1: int
      EntityID: EntityID }

    interface IRow with
        member this.Key =
            let (EntityID key) = this.EntityID
            key


let getNoun (entity: Entity) =
    entity.Attributes
    |> ValueOption.map (fun t -> t.Ancestry.Noun)
    |> ValueOption.defaultValue ""



let setID (entity: Entity) (entityID) =

    { entity with
        ID = entityID
        Attributes = entity.Attributes |> ValueOption.map (fun t -> { t with ID = entityID })
        FloorActor = entity.FloorActor |> ValueOption.map (fun t -> { t with ID = entityID })
        FloorLocation = entity.FloorLocation |> ValueOption.map (fun t -> { t with ID = entityID }) }

let setNoun (entity: Entity) (noun: string) =
    { entity with
        Attributes =
            entity.Attributes
            |> ValueOption.map (fun t ->
                { t with
                    EntityAttributes.Ancestry.Noun = noun }) }


type ExampleTable() as this =
    inherit Table<Entity, EntityID>(EntityID, (fun (EntityID value) -> value), EntityTable.entityTableEventToStepItems)

    let _multiIndexByName = new MultiIndex<Entity, string>(getNoun)

    do this.AddIndex(_multiIndexByName)

    member this.NameIndex = _multiIndexByName


let makeExampleItem (entityID: EntityID) =

    { Entity.ID = entityID
      Attributes =
        { ID = entityID
          Ancestry =
            { Adjective = "asdf"
              Description = "324"
              Noun = "Test1"
              Tags = Map.empty
              ID = AncestryID(5L)
              RequireTags = Map.empty }
          Heritage = []
          Tags = Map.empty }
        |> ValueSome
      FloorActor =
        { ID = entityID
          NextAction = ActionArchetype.CharacterEngineInput
          NextTick = 0L<TimeTick>
          TickActions = ActionArchetype.DefaultTickActions
          PlayerID = ValueNone }
        |> ValueSome
      FloorLocation =
        { ID = entityID
          Position = Morgemil.Math.Point.Identity
          FloorID = FloorID 2L }
        |> ValueSome }


[<Fact>]
let ``Can Add items to Index`` () =
    let exampleTable = new ExampleTable()

    Assert.Equal(EntityID 0L, Table.GenerateKey exampleTable)

    let exampleItem0 = makeExampleItem (EntityID 500L)
    Table.AddRow exampleTable exampleItem0

    let exampleItem1 = makeExampleItem (EntityID 501L)
    Table.AddRow exampleTable exampleItem1

    let exampleItem2 = makeExampleItem (EntityID 502L)
    Table.AddRow exampleTable exampleItem2

    Assert.Equal(EntityID 503L, Table.GenerateKey exampleTable)
    Assert.Equal(3, exampleTable |> Table.Items |> Seq.length)

    Assert.Throws<System.Exception>(fun () -> Table.GetRowByKey exampleTable (EntityID 123L) |> ignore)
    |> ignore

    Assert.Equal(exampleItem0, Table.GetRowByKey exampleTable (EntityID 500L))

    Assert.Equal(3, (MultiIndex.GetRowsByKey exampleTable.NameIndex "Test1") |> Seq.length)

    Assert.Empty(MultiIndex.GetRowsByKey exampleTable.NameIndex "Test1234234234")

    Table.RemoveRow exampleTable exampleItem0
    Assert.Equal(2, exampleTable |> Table.Items |> Seq.length)

    Assert.Throws<System.Exception>(fun () -> Table.GetRowByKey exampleTable (EntityID 500L) |> ignore)
    |> ignore

    Assert.Equal(2, (MultiIndex.GetRowsByKey exampleTable.NameIndex "Test1") |> Seq.length)

    let exampleItem3 = setNoun exampleItem1 "onetwo"
    Table.AddRow exampleTable exampleItem3

    let exampleItem4 = setNoun exampleItem2 "onetwo"
    Table.AddRow exampleTable exampleItem4

    Assert.Equal(exampleItem3, (Table.GetRowByKey exampleTable (EntityID 501L)))
    Assert.Empty(MultiIndex.GetRowsByKey exampleTable.NameIndex "Test1")

    Assert.Equal(2, (MultiIndex.GetRowsByKey exampleTable.NameIndex "onetwo") |> Seq.length)

    for index in [ 1L .. 20_000L ] do
        (setNoun (setID exampleItem2 (EntityID index)) (index.ToString()))
        |> Table.AddRow exampleTable

    Assert.Equal(20_000, exampleTable |> Table.Items |> Seq.length)

    for item in exampleTable |> Table.Items |> Seq.toList do
        setNoun item "asdf" |> Table.AddRow exampleTable

    Assert.Equal(20_000, exampleTable |> Table.Items |> Seq.length)

    for index in [ 1L .. 12_000L ] do
        index |> EntityID |> Table.RemoveRowByKey exampleTable

    Assert.Equal(8_000, exampleTable |> Table.Items |> Seq.length)

    for index in [ 1L .. 50_000L ] do
        index |> EntityID |> Table.RemoveRowByKey exampleTable

    Assert.Equal(0, exampleTable |> Table.Items |> Seq.length)

    let readOnlyTable =
        Table.CreateReadonlyTable (fun (EntityID value) -> value) [ exampleItem1; exampleItem2 ]

    Assert.Equal(2, readOnlyTable.Items |> Seq.length)

    Assert.Equal(exampleItem1, 501L |> EntityID |> Table.GetRowByKey readOnlyTable)

    Assert.Equal(exampleItem2, 502L |> EntityID |> Table.GetRowByKey readOnlyTable)



type ExampleRowNext =
    { Name: string
      EntityIDID: int64
      ForeignKeyID: int64 }

    interface IRow with
        member this.Key = this.EntityIDID


type ExampleRowNext234 =
    { Name: string
      EntityIDID: int64
      ForeignKeyID: int64 }

    interface IRow with
        member this.Key = this.EntityIDID

[<Fact>]
let ``Check Joins on readonlyArrays`` () =

    let exampleItem1 =
        { ExampleRow.Attribute1 = 0
          EntityID = EntityID(500L)
          Name = "Test1" }

    let exampleItem2 =
        { ExampleRow.Attribute1 = 23
          EntityID = EntityID(501L)
          Name = "Test1" }

    let exampleItem3 = { exampleItem2 with Name = "onetwo" }

    let row1 =
        { ExampleRowNext.Name = "one"
          EntityIDID = 234L
          ForeignKeyID = 500L }

    let row2 =
        { ExampleRowNext.Name = "two"
          EntityIDID = 235L
          ForeignKeyID = 501L }

    let row3 =
        { ExampleRowNext.Name = "two"
          EntityIDID = 235L
          ForeignKeyID = 0L }

    let exampleItem234 =
        { ExampleRowNext234.Name = "two"
          EntityIDID = 235L
          ForeignKeyID = 0L }

    let tableOne =
        Table.CreateReadonlyTable (fun (EntityID value) -> value) [ exampleItem1; exampleItem2; exampleItem3 ]

    let tableTwo =
        Table.CreateReadonlyTable (fun (EntityID value) -> value) [ row1; row2; row3 ]

    let tableThree =
        Table.CreateReadonlyTable (fun (EntityID value) -> value) [ exampleItem234 ]

    let joinedRows =
        TableQuery.LeftJoin tableTwo (fun t -> t.ForeignKeyID |> EntityID) tableOne
        |> Seq.toArray

    Assert.Equal(3, joinedRows.Length)

    let row, exampleItem = joinedRows[2]
    Assert.True(exampleItem.IsNone)
    Assert.Equal(row3, row)

    let row, exampleItem = joinedRows[1]
    Assert.Equal(exampleItem2, exampleItem.Value)
    Assert.Equal(row2, row)
