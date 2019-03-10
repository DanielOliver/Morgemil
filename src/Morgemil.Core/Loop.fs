namespace Morgemil.Core

open Morgemil.Math

type LoopEvent =
    | MoveWest
    | MoveEast
    | MoveNorth
    | MoveSouth

type Loop(characters: CharacterTable, tileMap: TileMap) =

    member this.Process(event: LoopEvent) =
        let vec1 =
            match event with
            | MoveWest -> Vector2i.create(-1, 0)
            | MoveNorth -> Vector2i.create(0, -1)
            | MoveSouth -> Vector2i.create(0, 1)
            | MoveEast -> Vector2i.create(1, 0)

        let moveCharacter = Table.Items characters |> Seq.head
        let blocksMovement = tileMap.Item(moveCharacter.Position + vec1) |> TileMap.blocksMovement
        if not blocksMovement then
            Table.AddRow characters {
                moveCharacter with
                    Position = moveCharacter.Position + vec1
            }

