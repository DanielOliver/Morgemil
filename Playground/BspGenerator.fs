namespace Morgemil.Test

open Morgemil.Map
open Morgemil.Math

module BspGenerator =
  ///The smallest room possible
  let MinRoomSize = Vector2i(23, 23)

  ///Average more towards this size in theory
  let TargetRoomSize = Vector2i(41, 41)

  type Tree =
    | Node of Tree * Tree
    | Leaf of Rectangle

  type Axis =
    | Horizontal
    | Vertical

  ///Choose the opisite Axis
  let Opposite(ax : Axis) =
    match ax with
    | Axis.Vertical -> Axis.Horizontal
    | Axis.Horizontal -> Axis.Vertical

  let CanDivideHorizontal(area : Rectangle) = area.Width > MinRoomSize.X * 2
  let CanDivideVertical(area : Rectangle) = area.Height > MinRoomSize.Y * 2
  let CanDivide(area : Rectangle) = CanDivideHorizontal area || CanDivideVertical area

  ///Chooses an axis to split on. Keeps ratios fairly square.
  let AxisToDivide (ax : Axis) (area : Rectangle) =
    match area with
    | _ when area.Height >= area.Width * 2 -> Axis.Vertical
    | _ when area.Width >= area.Height * 2 -> Axis.Horizontal
    | _ when not (CanDivideHorizontal area) -> Axis.Vertical
    | _ when not (CanDivideVertical area) -> Axis.Horizontal
    | _ -> Opposite(ax)

  ///Rectangle * Rectangle
  let Divide (area : Rectangle) (ax : Axis) (rng : RNG.DefaultRNG) =
    match ax with
    | Axis.Horizontal ->
      let rng_width = RNG.Range rng (MinRoomSize.X) (area.Width - MinRoomSize.X)
      let first = Rectangle(area.Position, Vector2i(rng_width, area.Height))
      let second =
        Rectangle(area.Position + Vector2i(rng_width, 0), area.Size - Vector2i(rng_width, 0))
      (first, second)
    | Axis.Vertical ->
      let rng_height = RNG.Range rng (MinRoomSize.Y) (area.Height - MinRoomSize.Y)
      let first = Rectangle(area.Position, Vector2i(area.Width, rng_height))
      let second =
        Rectangle(area.Position + Vector2i(0, rng_height), area.Size - Vector2i(0, rng_height))
      (first, second)

  ///Recursively divides an area into a Binary Space Partitioning Tree
  let rec BSP (rng : RNG.DefaultRNG) (area : Rectangle) (ax : Axis) =
    let prob =
      (decimal ((area.Size - MinRoomSize).Area) / decimal ((TargetRoomSize - MinRoomSize).Area))
    let divide = CanDivide(area) && RNG.Probability rng prob
    match divide with
    | false -> Tree.Leaf area
    | true ->
      let opAx = AxisToDivide ax area
      let (first, second) = Divide area opAx rng
      Tree.Node(BSP rng first opAx, BSP rng second opAx)

  //This is essentially a constant since it takes no arguments
  let DungeonGenerator =
    let dungeonFloor =
      TileDefinition(1, "Floor", "Dungeon floors are often trapped", false, true, TileType.Land)

    let BspResults =
      ///Inject the seed in the future
      let seed = System.Random().Next()
      BSP (RNG.SeedRNG(seed)) (Rectangle(Vector2i(0, 0), Vector2i(234, 124))) Axis.Horizontal

    ///flatterns a BSPTree into Rectangle List
    let rec flatten treeNode =
      match treeNode with
      | Tree.Leaf(rect) -> [ rect ]
      | Tree.Node(e1, e2) ->
        [ flatten e1
          flatten e2 ]
        |> List.concat

    ///(0,0) (90,0) (180,0)...
    let chunksToCreatePositions = flatten BspResults

    ///Given the corner of the chunk to create
    let CreateRoomChunk(roomArea : Rectangle) =
      ///border is empty tiles and the contained area is dungeon floor
      let ChooseTile(tilePosition : Vector2i) =
        match tilePosition with
        | _ when tilePosition.X = roomArea.Left || tilePosition.X = roomArea.Right
                 || tilePosition.Y = roomArea.Top || tilePosition.Y = roomArea.Bottom ->
          TileDefinition.Default
        | _ -> dungeonFloor

      ///Maps each coordinate in the room into a tile
      let tileArray =
        roomArea.Coordinates
        |> Seq.map (ChooseTile)
        |> Seq.toArray

      Chunk(roomArea, tileArray)

    chunksToCreatePositions |> Seq.map (CreateRoomChunk)
