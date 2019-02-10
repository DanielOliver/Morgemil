namespace Morgemil.Models

type Race = 
  { ID : RaceID
    ///Proper noun
    Noun : string
    ///Proper adjective
    Adjective : string
    ///User-visible description
    Description : string
    ///A list of Tags that this Race possesses, along with the Value(s)
    Tags : Map<TagType, Tag> }
