module Morgemil.Data.Validator
open Morgemil.Core
open Morgemil.Data.DTO
    
let private ExpectedUnique<'T> (currentItem: 'T) (property: 'T -> _) (propertyName: string) (items: 'T DtoValidResult list) : string option =
    let currentItemProperty = property currentItem
    items |> Seq.tryFind(fun x -> property x.Item = currentItemProperty) |> Option.map(fun duplicate -> sprintf "Expected Unique %s: %A" propertyName currentItemProperty)
    
let inline private DefinedEnum< ^T > (value: ^T): string option =
    if System.Enum.IsDefined(typeof< ^T>, value) then
        None
    else 
        sprintf "Value %A is not defined for enum %s" value typeof<'T>.Name |> Some

let private ValidateDtoTiles (item: DtoValidResult<Tile[]>): DtoValidResult<DtoValidResult<Tile>[]> * IReadonlyTable<Tile, int64> =
    let table: IReadonlyTable<Tile, int64> = item.Item |> Table.CreateReadonlyTable id    
    let validatedItems =
        item.Item
        |> Array.fold (fun (acc: List<DtoValidResult<Tile>>) (element: Tile) ->
            let errors =
                [
                    ExpectedUnique element (fun x -> x.ID) "TileID" acc
                    DefinedEnum element.TileType
                ] |> List.choose id
            {
                DtoValidResult.Item = element
                Errors = errors
                Success = errors |> List.isEmpty
            } :: acc) []
        |> List.rev
        |> List.toArray
    {
        Item = validatedItems
        Success = validatedItems |> Seq.forall(fun x -> x.Success)
        Errors = []
    }, table
    

let ValidateDtos (phase0: RawDtoPhase0): RawDtoPhase1 =
    let (tileResults, tileTable) = ValidateDtoTiles phase0.Tiles;
    
    {
        RawDtoPhase1.Tiles = tileResults
    }
    
