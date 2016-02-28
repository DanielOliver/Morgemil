namespace Morgemil.Core

open Morgemil.Math

///Wight, human, etc.
type Race = 
  { Id : int
    ///Proper noun
    Noun : string
    ///Proper adjective
    Adjective : string
    ///User-visible description
    Description : string }
