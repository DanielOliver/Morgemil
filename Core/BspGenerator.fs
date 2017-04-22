namespace Morgemil.Core

open Morgemil.Core
open Morgemil.Math

type private Tree = 
  | Node of Tree * Tree
  | Leaf of Rectangle

/// Subdivides a rectangular area into pseudo-random rectangles
type BspGenerator(minRoomSize : Vector2i, maxRoomSize : Vector2i, dungeonSize : Vector2i) = 
  let canDivideHorizontal (area : Rectangle) = area.Width > minRoomSize.X * 2
  let canDivideVertical (area : Rectangle) = area.Height > minRoomSize.Y * 2
  let canDivide (area : Rectangle) = canDivideHorizontal area || canDivideVertical area
  
  ///Chooses an axis to split on. Keeps ratios fairly square.
  let axisToDivide (ax : Axis) (area : Rectangle) = 
    match area with
    | _ when not (canDivideHorizontal area) -> Axis.Vertical
    | _ when not (canDivideVertical area) -> Axis.Horizontal
    | _ when area.Height >= area.Width * 2 -> Axis.Vertical
    | _ when area.Width >= area.Height * 2 -> Axis.Horizontal
    | _ -> ax.Opposite
  
  ///Rectangle * Rectangle
  let divide (area : Rectangle) (ax : Axis) rng = 
    match ax with
    | Axis.Horizontal -> 
      let rng_width = RNG.Range rng (minRoomSize.X) (area.Width - minRoomSize.X)
      (Rectangle.create(area.Position, Vector2i.create(rng_width, area.Height)), 
       Rectangle.create(area.Position + Vector2i.create(rng_width, 0), area.Size - Vector2i.create(rng_width, 0)))
    | Axis.Vertical -> 
      let rng_height = RNG.Range rng (minRoomSize.Y) (area.Height - minRoomSize.Y)
      (Rectangle.create(area.Position, Vector2i.create(area.Width, rng_height)), 
       Rectangle.create(area.Position + Vector2i.create(0, rng_height), area.Size - Vector2i.create(0, rng_height)))
  
  ///Recursively divides an area into a Binary Space Partitioning Tree
  let rec bsp rng (area : Rectangle) (ax : Axis) = 
    let prob = (decimal ((area.Size - minRoomSize).Area) / decimal ((maxRoomSize - minRoomSize).Area)) / 2.0m
    match canDivide (area) && RNG.Probability rng prob with
    | false -> Tree.Leaf area
    | true -> 
      let opAx = axisToDivide ax area
      let (first, second) = divide area opAx rng
      Tree.Node(bsp rng first opAx, bsp rng second opAx)
  
  ///flatterns a BSPTree into Rectangle List
  let rec flattenBSPTree treeNode = 
    match treeNode with
    | Tree.Leaf(rect) -> [ rect ]
    | Tree.Node(e1, e2) -> 
      [ flattenBSPTree e1
        flattenBSPTree e2 ]
      |> List.concat
  
  /// Returns the list of rectangles BSP subdivided the area into.
  member this.GenerateRoomDivides rng = bsp rng (Rectangle.create(dungeonSize)) Axis.Horizontal |> flattenBSPTree
