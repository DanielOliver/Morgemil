namespace Morgemil.Logic

module Extensions = 
  type Map<'K, 'T when 'K : comparison> with
    member this.Replace(key, value) = this.Remove(key).Add(key, value)
    member this.Replace(key, mapFunction) = 
      match this.TryFind key with
      | Some(old) -> this.Replace(key, (mapFunction old))
      | _ -> this
