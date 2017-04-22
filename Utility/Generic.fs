module Morgemil.Utility.Generic

open Morgemil.Models

let inline IsPlayerOption< ^a when ^a: (member Tags: Map<TagType, Tag>)> item =
  (^a : (member Tags: Map<TagType, Tag>) item).ContainsKey(TagType.PlayerOption)

let IsValidArrayIndex (array: _ []) index =
  index >= 0 && index < array.Length
  