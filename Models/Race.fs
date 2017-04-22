namespace Morgemil.Models

type Race = 
  { ID : int
    ///Proper noun
    Noun : string
    ///Proper adjective
    Adjective : string
    ///User-visible description
    Description : string
    ///The available Racial Modifiers
    RaceModifiers: RaceModifierRatio []
    ///A list of Tags that this Race possesses, along with the Value(s)
    Tags : Map<TagType, Tag> }
