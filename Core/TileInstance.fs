namespace Morgemil.Map

//This defines a live tile. Refer to Tile Definition for generics
type TileInstance(position : Morgemil.Math.Vector2.Vector2i, definition : TileDefinition) = 
  //Does position need to be stored?
  member this.Position = position
  member this.Definition = definition
