namespace Morgemil.Logic

type IdentityPool<'U>(initial, fromInt : int -> 'U, toInt : 'U -> int) = 
  let mutable _pool : Set<int> = initial
  
  let mutable _available : Set<int> = 
    match initial.IsEmpty with
    | true -> Set.empty
    | _ -> set [ (_pool.MinimumElement)..(_pool.MaximumElement) ] |> Set.difference (_pool)
  
  member this.Items = seq _pool
  
  member this.Generate() = 
    fromInt (match _available.IsEmpty with
             | true -> 
               match _pool.IsEmpty with
               | true -> 
                 _pool <- _pool.Add(0)
                 0
               | _ -> 
                 let result = _pool.MaximumElement + 1
                 _pool <- _pool.Add(result)
                 result
             | _ -> 
               let result = _available.MinimumElement
               _pool <- _pool.Add(result)
               _available <- _available.Remove(result)
               result)
  
  member this.Free id = 
    let intId = toInt id
    _pool <- _pool.Remove(intId)
    if _pool.Count = 0 then _available <- Set.empty
    else if _pool.MaximumElement <= intId then 
      _available <- _available |> Set.filter (fun t -> t <= _pool.MaximumElement)
    else _available <- _available.Add(intId)
