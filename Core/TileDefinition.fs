namespace Morgemil.Map

type TileType =
  | Void = 0
  | Land = 1
  | Water = 2

/// <summary>
/// This defines a tile. Each tile instance will hold a reference to one of these as a base attribute.
/// </summary>
type TileDefinition(id : int, name : string, description : string, blocksMovement : bool, blocksSight : bool, tileType : TileType) =

  /// <summary>
  /// Use this in file storage. When saving a chunk or map, use this ID.
  /// </summary>
  member this.ID = id

  /// <summary>
  /// A short name. eg: "Lush Grass"
  /// </summary>
  member this.Name = name

  /// <summary>
  /// A long description. eg: "Beware the burning sand. Scorpions and asps make their home here."
  /// </summary>
  member this.Description = description

  /// <summary>
  /// If true, this tile ALWAYS blocks ALL movement by ANYTHING.
  /// </summary>
  member this.BlocksMovement = blocksMovement

  /// <summary>
  /// If true, this tile ALWAYS blocks ALL Line-Of-Sight of ANYTHING.
  /// </summary>
  member this.BlocksSight = blocksSight

  /// <summary>
  /// The tile type determines some things like if they can breath or not.
  /// </summary>
  member this.Type = tileType

  /// <summary>
  /// A default, minimal definition. Could be used as the edge of the map blackness?
  /// </summary>
  static member Default = TileDefinition(0, "Empty", "Nothing", true, true, TileType.Void)
