namespace Morgemil.Math

open System.Collections.Generic

module Graph =
  ///ID and an associated data
  type Vertex<'T>(id : int, data : 'T) =
    member this.Id = id
    member this.Data = data

    interface System.IComparable with
      member this.CompareTo other =
        match other with
        | :? Vertex<'T> as y -> (compare this.Id y.Id)
        | _ -> invalidArg "Vertex<_>" "cannot compare values of different types"

    override this.Equals other =
      match other with
      | :? Vertex<'T> as y -> (compare this.Id y.Id) = 0
      | _ -> false

    override this.GetHashCode() = hash (this)

  ///Undirected edge to use in a UndirectedGraph
  type UndirectedEdge<'T>(ver1 : Vertex<'T>, ver2 : Vertex<'T>) =

    member this.First =
      match ver1.Id < ver2.Id with
      | true -> ver1
      | false -> ver2

    member this.Second =
      match ver1.Id < ver2.Id with
      | false -> ver1
      | true -> ver2

  type VertexList<'T> = list<Vertex<'T>>

  type Regions<'T>(distinct : list<VertexList<'T>>) =

    ///The distinct regions
    member this.Distinct = distinct

    ///Tries to connect an edge. Returns this if already in same region.
    member this.Connect(edge : UndirectedEdge<_>) =
      let change, unchange =
        distinct
        |> List.partition
             (fun verList -> verList |> List.exists (fun x -> x = edge.First || x = edge.Second))
      match change.Length with
      | 0 | 1 -> this
      | _ -> Regions((change |> List.concat) :: unchange)

  ///List of vertices. List of edges. Distinct regions.
  type UndirectedGraph<'T>(vertices : VertexList<'T>, edges : list<UndirectedEdge<'T>>, regions : Regions<'T>) =
    member this.Vertices = vertices
    member this.Edges = edges
    member this.Regions = regions.Distinct

    ///Initial empty construction without edges
    new(vertices : VertexList<'T>) =
      UndirectedGraph(vertices, list.Empty, Regions(vertices |> List.map (fun ver -> [ ver ])))

    member this.Add(newEdge) =
      let newEdgeList = newEdge :: edges
      UndirectedGraph(vertices, newEdgeList, regions.Connect newEdge)
