module Morgemil.Utility.JsonHelper.Tests

open NUnit.Framework
open FSharp.Data
open Morgemil.Utility.JsonHelper

[<Test>]
let ``Json ComputationExpression reads values``() =
    let baseValue =
        JsonValue.Record(
            [|
            "Record1", JsonValue.Record(
                [|
                "IsTrue", JsonValue.Boolean true
                "IsDog", JsonValue.String "Dog"
                |])
            "IsFalse", JsonValue.Boolean false
            "IsNine", JsonValue.Number 9m
            |])
    let result = json baseValue {
        let! nine = ("IsNine", AsInteger)
        let! isFalse = ("IsFalse", AsBoolean)
        Assert.IsFalse isFalse
        let! record1 = "record1"
        let! (isTrue, isDog) = json record1 {
            let! isTrue = ("IsTrue", AsBoolean)
            let! isDog = ("IsDog", AsString)
            Assert.AreEqual("Dog", isDog)
            return (isTrue, isDog)
        }
        Assert.AreEqual("Dog", isDog)
        
        let! isAbsent = Optional("CantFindThis", AsString)
        return (nine)
    }
    match result with
    | Ok data ->
        Assert.Fail("Shouldn't succeed in parsing above")
    | Error err ->
        Assert.Pass() 
