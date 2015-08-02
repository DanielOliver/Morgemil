namespace Morgemil.Map

type TileType = 
  | Void = 0
  | Land = 1
  | Water = 2

/// This defines a tile. Each tile instance will hold a reference to one of these as a base attribute.
type TileDefinition = 
  { ///Use this in file storage. When saving a level or map, use this ID.
    Id : int
    /// A short name. eg: "Lush Grass"
    Name : string
    ///A long description. eg: "Beware the burning sand. Scorpions and asps make their home here."
    Description : string
    ///If true, this tile ALWAYS blocks ALL movement by ANYTHING.
    BlocksMovement : bool
    ///If true, this tile ALWAYS blocks ALL Line-Of-Sight of ANYTHING.
    BlocksSight : bool
    ///The tile type determines some things like if they can breath or not.
    TileType : TileType }
  
  /// <summary>
  /// A default, minimal definition. Could be used as the edge of the map blackness?
  /// </summary>
  static member Default = 
    { Id = 0
      Name = "Empty"
      Description = ""
      BlocksMovement = true
      BlocksSight = true
      TileType = TileType.Void }
  
  static member IsDefault(tile : TileDefinition) = (tile.Id = TileDefinition.Default.Id)
