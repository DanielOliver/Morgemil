﻿namespace Morgemil.Test

open Morgemil.Math

type Walkabout(dungeon : Morgemil.Map.Chunk, player : Morgemil.Game.Person) =
  member this.Dungeon = dungeon
  member this.Player = player
  member this.Act(act : Morgemil.Game.Actions) =
    let offset =
      match act with
      | Morgemil.Game.MoveEast -> Vector2i(1, 0)
      | Morgemil.Game.MoveWest -> Vector2i(-1, 0)
      | Morgemil.Game.MoveNorth -> Vector2i(0, 1)
      | Morgemil.Game.MoveSouth -> Vector2i(0, -1)
    Walkabout(dungeon,
              { Id = player.Id
                //Body = player.Body
                Position = player.Position + offset })
