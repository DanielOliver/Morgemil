namespace Morgemil.Map

open Morgemil.Math

type private Tree = 
  | Node of Tree * Tree
  | Leaf of Rectangle

/// Subdivides a rectangular area into pseudo-random rectangles
type BspGenerator(MinRoomSize : Vector2i, MaxRoomSize : Vector2i, DungeonSize : Vector2i) = 
  let CanDivideHorizontal(area : Rectangle) = area.Width > MinRoomSize.X * 2
  let CanDivideVertical(area : Rectangle) = area.Height > MinRoomSize.Y * 2
  let CanDivide(area : Rectangle) = CanDivideHorizontal area || CanDivideVertical area
  
  ///Chooses an axis to split on. Keeps ratios fairly square.
  let AxisToDivide (ax : Axis) (area : Rectangle) = 
    match area with
    | _ when not (CanDivideHorizontal area) -> Axis.Vertical
    | _ when not (CanDivideVertical area) -> Axis.Horizontal
    | _ when area.Height >= area.Width * 2 -> Axis.Vertical
    | _ when area.Width >= area.Height * 2 -> Axis.Horizontal
    | _ -> ax.Opposite
  
  ///Rectangle * Rectangle
  let Divide (area : Rectangle) (ax : Axis) rng = 
    match ax with
    | Axis.Horizontal -> 
      let rng_width = RNG.Range rng (MinRoomSize.X) (area.Width - MinRoomSize.X)
      (Rectangle(area.Position, Vector2i(rng_width, area.Height)), 
       Rectangle(area.Position + Vector2i(rng_width, 0), area.Size - Vector2i(rng_width, 0)))
    | Axis.Vertical -> 
      let rng_height = RNG.Range rng (MinRoomSize.Y) (area.Height - MinRoomSize.Y)
      (Rectangle(area.Position, Vector2i(area.Width, rng_height)), 
       Rectangle(area.Position + Vector2i(0, rng_height), area.Size - Vector2i(0, rng_height)))
  
  ///Recursively divides an area into a Binary Space Partitioning Tree
  let rec BSP rng (area : Rectangle) (ax : Axis) = 
    let prob = (decimal ((area.Size - MinRoomSize).Area) / decimal ((MaxRoomSize - MinRoomSize).Area)) / 2.0m
    match CanDivide(area) && RNG.Probability rng prob with
    | false -> Tree.Leaf area
    | true -> 
      let opAx = AxisToDivide ax area
      let (first, second) = Divide area opAx rng
      Tree.Node(BSP rng first opAx, BSP rng second opAx)
  
  ///flatterns a BSPTree into Rectangle List
  let rec FlattenBSPTree treeNode = 
    match treeNode with
    | Tree.Leaf(rect) -> [ rect ]
    | Tree.Node(e1, e2) -> 
      [ FlattenBSPTree e1
        FlattenBSPTree e2 ]
      |> List.concat
  
  /// Returns the list of rectangles BSP subdivided the area into.
  member this.GenerateRoomDivides rng = BSP rng (Rectangle(DungeonSize)) Axis.Horizontal |> FlattenBSPTree
