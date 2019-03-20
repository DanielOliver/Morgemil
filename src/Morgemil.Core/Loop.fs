namespace Morgemil.Core

open Morgemil.Models

type Loop(characters: CharacterTable, tileMap: TileMap) =

    member this.ProcessRequest(event: ActionRequest): ActionEvent seq =
        match event with
        | Move (direction) ->

            let moveCharacter = Table.Items characters |> Seq.head
            let oldPosition = moveCharacter.Position 
            let newPosition = oldPosition + direction
            let blocksMovement = tileMap.Item(newPosition) |> TileMap.blocksMovement
            if not blocksMovement then
                Table.AddRow characters {
                    moveCharacter with
                        Position = newPosition
                }
            {
                CharacterID = moveCharacter.ID
                OldPosition = oldPosition
                NewPosition = newPosition
            }
            |> ActionEvent.AfterMove
            |> Seq.singleton
            
            
               

