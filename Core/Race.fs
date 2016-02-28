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
    Description : string
    ///Normal "bodies" fit in one tile (1,1). Bosses and the largest entities can take up multiple tiles.
    Size : Vector2i }
  ///The ID is also the array indices
  static member Lookup = 
    [| { Id = 0
         Noun = "Human"
         Adjective = "Human"
         Description = "Humans are no longer the primary race."
         Size = Vector2i(1) }
       { Id = 1
         Noun = "Dwarf"
         Adjective = "Dwarven"
         Description = "Tribal, rampaging war machines"
         Size = Vector2i(1) }
       { Id = 2
         Noun = "Goblin"
         Adjective = "Goblin"
         Description = "A fun loving, daring, matriarchal people"
         Size = Vector2i(1) }
       { Id = 3
         Noun = "Dragon"
         Adjective = "Dragon"
         Description = "Fear giant lizards that eat everything"
         Size = Vector2i(4) } |]
