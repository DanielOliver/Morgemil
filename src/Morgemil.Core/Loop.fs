namespace Morgemil.Core

open Morgemil.Models

type Loop(characters: CharacterTable, tileMap: TileMap) =

    member this.ProcessRequest(event: ActionRequest): Character Step list =
        Table.ClearHistory characters
        EventHistoryBuilder characters {
            match event with
            | Move (direction) ->

                let moveCharacter = Table.Items characters |> Seq.head
                let oldPosition = moveCharacter.Position 
                let newPosition = oldPosition + direction
                let blocksMovement = tileMap.Item(newPosition) |> TileMap.blocksMovement
                if blocksMovement then
                    yield
                        {
                            CharacterID = moveCharacter.ID
                            OldPosition = oldPosition
                            RequestedPosition = newPosition
                        }
                        |> ActionEvent.RefusedMove
                else 
                    Table.AddRow characters {
                        moveCharacter with
                            Position = newPosition
                    }
                    yield
                        {
                            CharacterID = moveCharacter.ID
                            OldPosition = oldPosition
                            NewPosition = newPosition
                        }
                        |> ActionEvent.AfterMove
            if Table.HasHistory characters then 
                yield ActionEvent.Empty
        }
