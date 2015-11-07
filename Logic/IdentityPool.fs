namespace Morgemil.Logic

type IdentityPool(initial) = 
  let mutable _pool : Set<int> = initial
  
  let mutable _available : Set<int> = 
    match initial.IsEmpty with
    | true -> Set.empty
    | _ -> set [ (_pool.MinimumElement)..(_pool.MaximumElement) ] |> Set.difference (_pool)
  
  member this.Generate() = 
    match _available.IsEmpty with
    | true -> 
      let result = _pool.MaximumElement + 1
      _pool <- _pool.Add(result)
      result
    | _ -> 
      let result = _available.MinimumElement
      _pool <- _pool.Add(result)
      _available <- _available.Remove(result)
      result
  
  member this.Free id = 
    _pool <- _pool.Remove(id)
    if _pool.MaximumElement <= id then _available <- _available |> Set.filter (fun t -> t <= _pool.MaximumElement)
