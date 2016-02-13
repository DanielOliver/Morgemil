namespace Morgemil.Logic

type IdentityPool<'U when 'U : comparison>(initial, fromInt : int -> 'U, toInt : 'U -> int) = 
  let mutable _pool : Set<int> = initial
  let mutable _dead : Set<int> = Set.empty
  let getAvailable() = set [ (_pool.MinimumElement)..(_pool.MaximumElement) ] |> Set.difference (_pool)
  let mutable _available : Set<int> = getAvailable()
  member this.Items = _pool |> Seq.map (fromInt)
  member this.Raw = seq _pool
  
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
  
  member this.Kill id = _dead <- _dead.Add(toInt id)
  
  member this.Free() = 
    let toRemove = _dead
    _dead <- Set.empty
    _pool <- _pool |> Set.difference _dead
    _available <- getAvailable()
    toRemove |> Set.map (fromInt)
  
  member this.IsDead id = _dead.Contains(toInt id)
