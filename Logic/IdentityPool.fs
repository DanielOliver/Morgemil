namespace Morgemil.Logic

type IdentityStatus = 
  | Live
  | Dead

type IdentityPool<'U>(initial, fromInt : int -> 'U, toInt : 'U -> int) = 
  let mutable _pool : Set<int> = initial
  let mutable _dead : Set<int> = Set.empty
  
  let getAvailable() = 
    match initial.IsEmpty with
    | true -> Set.empty
    | _ -> set [ (_pool.MinimumElement)..(_pool.MaximumElement) ] |> Set.difference (_pool)
  
  let mutable _available : Set<int> = getAvailable()
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
  
  member this.Status id = 
    let intId = toInt id
    if _dead.Contains(intId) then IdentityStatus.Dead
    else IdentityStatus.Live
  
  ///Marks this entity to be freed next update.
  member this.Free id = 
    let intId = toInt id
    _dead <- _dead.Add(intId)
  
  member this.Update() = 
    if not (_dead.IsEmpty) then 
      _pool <- _pool - _dead
      _available <- getAvailable()
