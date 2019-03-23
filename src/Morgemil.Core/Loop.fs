namespace Morgemil.Core

open Morgemil.Models

type Loop(characters: CharacterTable, tileMap: TileMap) =

    member this.ProcessRequest(event: ActionRequest): Character Step list =
        Table.ClearHistory characters
        EventHistoryBuilder characters {
            match event with
            | ActionRequest.Move (characterID, direction) ->
                match characterID |> Table.TryGetRowByKey characters with
                | None -> ()
                | Some moveCharacter ->
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
            | ActionRequest.GoToNextLevel (characterID) ->
                match characterID |> Table.TryGetRowByKey characters with
                | None -> ()
                | Some moveCharacter ->
                    if tileMap.[moveCharacter.Position] |> TileMap.isExitPoint then
                        let items = characters
                                    |> Table.Items
                                    |> Seq.toArray
                        items
                        |> Seq.map(fun t -> {
                                t with
                                    Position = tileMap.EntryPoints |> Seq.head
                            })
                        |> Seq.iter (Table.AddRow characters)

                        yield
                            {
                                Characters =
                                    characters
                                    |> Table.Items
                                    |> Seq.map(fun t -> {
                                            t with
                                                Position = tileMap.EntryPoints |> Seq.head
                                        })
                                    |> Array.ofSeq
                                TileMapData = tileMap.TileMapData
                            }
                            |> ActionEvent.MapChange


            if Table.HasHistory characters then 
                yield ActionEvent.Empty
        }
