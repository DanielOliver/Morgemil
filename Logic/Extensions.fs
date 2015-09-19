namespace Morgemil.Logic

module Extensions = 
  type Map<'K, 'T when 'K : comparison> with
    member this.Replace key value = this.Remove(key).Add(key, value)
