namespace Morgemil.Map

open Morgemil.Math

module BspGenerator =
  ///The smallest room possible
  let private MinRoomSize = Vector2i(55, 55)

  ///Average more towards this size in theory
  let private TargetRoomSize = Vector2i(60, 60)

  type private Tree =
    | Node of Tree * Tree
    | Leaf of Rectangle

  type private Axis =
    | Horizontal
    | Vertical

  ///Choose the opisite Axis
  let private Opposite(ax : Axis) =
    match ax with
    | Axis.Vertical -> Axis.Horizontal
    | Axis.Horizontal -> Axis.Vertical

  let private CanDivideHorizontal(area : Rectangle) = area.Width > MinRoomSize.X * 2
  let private CanDivideVertical(area : Rectangle) = area.Height > MinRoomSize.Y * 2
  let private CanDivide(area : Rectangle) = CanDivideHorizontal area || CanDivideVertical area

  ///Chooses an axis to split on. Keeps ratios fairly square.
  let private AxisToDivide (ax : Axis) (area : Rectangle) =
    match area with
    | _ when not (CanDivideHorizontal area) -> Axis.Vertical
    | _ when not (CanDivideVertical area) -> Axis.Horizontal
    | _ when area.Height >= area.Width * 2 -> Axis.Vertical
    | _ when area.Width >= area.Height * 2 -> Axis.Horizontal
    | _ -> Opposite(ax)

  ///Rectangle * Rectangle
  let private Divide (area : Rectangle) (ax : Axis) rng =
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
  let rec private BSP rng (area : Rectangle) (ax : Axis) =
    let prob =
      (decimal ((area.Size - MinRoomSize).Area) / decimal ((TargetRoomSize - MinRoomSize).Area))
      / 2.0m
    let divide = CanDivide(area) && RNG.Probability rng prob
    match divide with
    | false -> Tree.Leaf area
    | true ->
      let opAx = AxisToDivide ax area
      let (first, second) = Divide area opAx rng
      Tree.Node(BSP rng first opAx, BSP rng second opAx)

  ///flatterns a BSPTree into Rectangle List
  let rec private FlattenBSPTree treeNode =
    match treeNode with
    | Tree.Leaf(rect) -> [ rect ]
    | Tree.Node(e1, e2) ->
      [ FlattenBSPTree e1
        FlattenBSPTree e2 ]
      |> List.concat

  /// <summary>
  /// Returns the list of rectangles BSP subdivided the area into.
  /// </summary>
  /// <param name="rngSeed"></param>
  /// <param name="dungeonSize"></param>
  let GenerateRoomDivides rng dungeonSize =
    BSP rng (Rectangle(dungeonSize)) Axis.Horizontal |> FlattenBSPTree
