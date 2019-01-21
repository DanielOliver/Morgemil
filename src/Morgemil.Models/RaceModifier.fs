namespace Morgemil.Models 

type RaceModifier =
  { ID: int
    ///Proper noun
    Noun: string
    ///Proper adjective
    Adjective: string
    ///User-visible description
    Description: string
    ///A list of Tags that this Race Modifier possesses, along with the Value(s)
    Tags : Map<TagType, Tag> }

