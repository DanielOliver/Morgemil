module Morgemil.Utility.WorldLoader

open Morgemil.Core
open Morgemil.Logic
open Newtonsoft.Json

let SaveWorld(filename : string, world : World) = 
  let json = JsonConvert.SerializeObject(world.Entities.Components)
  System.IO.File.WriteAllText(filename, json)

let OpenWorld(filename : string) = 
  let json = System.IO.File.ReadAllText(filename)
  let save = JsonConvert.DeserializeObject<seq<Component>>(json)
  World(Level.Empty, save)
