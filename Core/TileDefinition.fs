namespace Morgemil.Map

//This defines a tile. Each tile instance will hold a reference to one of these.
type TileDefinition(id : int, name : string, description : string, blocksMovement : bool, blocksSight : bool) = 
  member this.ID = id
  member this.Name = name
  member this.Description = description
  member this.BlocksMovement = blocksMovement
  member this.BlocksSight = blocksSight
  static member Default = TileDefinition(0, "Empty", "Nothing", true, true)
