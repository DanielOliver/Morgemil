module Morgemil.Utility.WorldLoader

open Morgemil.Core
open Morgemil.Logic
open Morgemil.Math
open Newtonsoft.Json

type LevelSerialized = 
  { Tiles : int array
    TileModifiers : List<TileModifier>
    Depth : int
    Area : Rectangle }
  
  static member FromLevel(level : Level) = 
    { LevelSerialized.Area = level.Area
      Depth = level.Depth
      TileModifiers = level.TileModifiers
      Tiles = 
        level.Tiles
        |> Seq.map (fun t -> t.Id)
        |> Seq.toArray }
  
  member this.ToLevel() = 
    { Level.Depth = this.Depth
      Tiles = 
        this.Tiles
        |> Seq.map (fun t -> Tiles.Lookup.[t])
        |> Seq.toArray
      Area = this.Area
      TileModifiers = this.TileModifiers }

type WorldSerialized = 
  { LevelSerialized : LevelSerialized
    Components : seq<Component>
    CurrentTime : decimal<GameTime> }

let SaveWorld(filename : string, world : World) = 
  let write = 
    { WorldSerialized.Components = world.Entities.Components
      LevelSerialized = LevelSerialized.FromLevel(world.Level)
      CurrentTime = world.CurrentTime }
  
  let json = JsonConvert.SerializeObject(write)
  System.IO.File.WriteAllText(filename, json)

let OpenWorld(filename : string) = 
  let json = System.IO.File.ReadAllText(filename)
  let save = JsonConvert.DeserializeObject<WorldSerialized>(json)
  World(save.LevelSerialized.ToLevel(), save.Components, save.CurrentTime)
