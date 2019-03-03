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
        
let private ExistsInTable (id: int64) (propertyName: string) (table: IReadonlyTable<_, int64>) : string option =
    match table.TryGetRow id with
    | Some _ -> None
    | None -> sprintf "%s with ID %i doesn't exist" propertyName id |> Some
    
let private ExistsInTableIfValue (id: System.Nullable<int64>) (propertyName: string) (table: IReadonlyTable<_, int64>) : string option =
    match id.HasValue with
    | false -> None
    | true ->
        match table.TryGetRow id.Value with
        | Some _ -> None
        | None -> sprintf "%s with ID %i doesn't exist" propertyName id.Value |> Some

let private AllExistsInTable (ids: int64 list) (propertyName: string) (table: IReadonlyTable<_, int64>) : string option =
    ids
    |> List.filter(table.TryGetRow >> Option.isNone)
    |> (fun t -> System.String.Join(",", t))
    |> function
       | x when x.Length = 0 -> None
       | x -> sprintf "%s with IDs [ %s ] doesn't exist" propertyName x |> Some
    
    
let private ValidateItems<'T> (getItemErrors: List<DtoValidResult<'T> > -> 'T -> string option list) (items: 'T []) =
    items
    |> Array.fold (fun (acc: List<DtoValidResult<'T>>) (element: 'T) ->
        let errors =
            getItemErrors acc element |> List.choose id
        {
            DtoValidResult.Item = element
            Errors = errors
            Success = errors |> List.isEmpty
        } :: acc) []
    |> List.rev
    |> List.toArray
    
    
    
    

let private ValidateDtoTiles (item: DtoValidResult<Tile[]>): DtoValidResult<DtoValidResult<Tile>[]> * IReadonlyTable<Tile, int64> =
    let table = item.Item |> Table.CreateReadonlyTable id    
    let validatedItems =
        item.Item
        |> ValidateItems (fun acc element ->
            [
                ExpectedUnique element (fun x -> x.ID) "TileID" acc
                DefinedEnum element.TileType
            ]
        )        
    {
        Item = validatedItems
        Success = validatedItems |> Seq.forall(fun x -> x.Success)
        Errors = []
    }, table


let private ValidateDtoTileFeatures (item: DtoValidResult<TileFeature[]>) (tileTable: IReadonlyTable<Tile, int64>): DtoValidResult<DtoValidResult<TileFeature>[]> * IReadonlyTable<TileFeature, int64> =
    let table = item.Item |> Table.CreateReadonlyTable id    
    let validatedItems =
        item.Item
        |> ValidateItems (fun acc element ->
            [
                ExpectedUnique element (fun x -> x.ID) "TileFeatureID" acc
                tileTable |> AllExistsInTable element.PossibleTiles "Tiles"
            ]
        )
    {
        Item = validatedItems
        Success = validatedItems |> Seq.forall(fun x -> x.Success)
        Errors = []
    }, table
    
let private ValidateDtoRaces (item: DtoValidResult<Race[]>) (raceModifierTable: IReadonlyTable<RaceModifier, int64>): DtoValidResult<DtoValidResult<Race>[]> * IReadonlyTable<Race, int64> =
    let table  = item.Item |> Table.CreateReadonlyTable id    
    let validatedItems =
        item.Item
        |> ValidateItems (fun acc element ->
            [
                ExpectedUnique element (fun x -> x.ID) "RaceID" acc
                raceModifierTable |> AllExistsInTable element.PossibleRaceModifiers "RaceModifiers"
            ]
        )
    {
        Item = validatedItems
        Success = validatedItems |> Seq.forall(fun x -> x.Success)
        Errors = []
    }, table
    
let private ValidateDtoRaceModifiers (item: DtoValidResult<RaceModifier[]>) : DtoValidResult<DtoValidResult<RaceModifier>[]> * IReadonlyTable<RaceModifier, int64> =
    let table = item.Item |> Table.CreateReadonlyTable id    
    let validatedItems =
        item.Item
        |> ValidateItems (fun acc element ->
            [
                ExpectedUnique element (fun x -> x.ID) "RaceModifierID" acc
            ]
        )
    {
        Item = validatedItems
        Success = validatedItems |> Seq.forall(fun x -> x.Success)
        Errors = []
    }, table
    
    
let private ValidateDtoRaceModifierLinks (item: DtoValidResult<RaceModifierLink[]>) (raceTable: IReadonlyTable<Race, int64>) (raceModifierTable: IReadonlyTable<RaceModifier, int64>) : DtoValidResult<DtoValidResult<RaceModifierLink>[]> * IReadonlyTable<RaceModifierLink, int64> =
    let table = item.Item |> Table.CreateReadonlyTable id
    let validatedItems =
        item.Item
        |> ValidateItems (fun acc element ->
            [
                ExpectedUnique element (fun x -> x.ID) "RaceModifierLinkID" acc
                raceTable |> ExistsInTable element.RaceID "Race"
                raceModifierTable |> ExistsInTableIfValue element.RaceModifierID "RaceModifier"
            ]
        )
    {
        Item = validatedItems
        Success = validatedItems |> Seq.forall(fun x -> x.Success)
        Errors = []
    }, table
    
let private ValidateDtoItems (item: DtoValidResult<Item[]>) : DtoValidResult<DtoValidResult<Item>[]> * IReadonlyTable<Item, int64> =
    let table = item.Item |> Table.CreateReadonlyTable id
    let validatedItems =
        item.Item
        |> ValidateItems (fun acc element ->
            let itemTypeError =
                match element.ItemType, element.Weapon, element.Wearable, element.Consumable with
                | ItemType.Weapon, x, _, _ when not x.IsEmpty -> None
                | ItemType.Wearable, _, x, _ when not x.IsEmpty -> None
                | ItemType.Consumable, _, _, x when not x.IsEmpty -> None
                | _ -> sprintf "Expected ItemType %A to have associated info" element.ItemType |> Some
            
            [
                ExpectedUnique element (fun x -> x.ID) "ItemID" acc
                DefinedEnum element.ItemType
                itemTypeError
            ]
        )
    {
        Item = validatedItems
        Success = validatedItems |> Seq.forall(fun x -> x.Success)
        Errors = []
    }, table  



let ValidateDtos (phase0: RawDtoPhase0): RawDtoPhase1 =
    let (tileResults, tileTable) = ValidateDtoTiles phase0.Tiles
    
    let (tileFeatureResults, tileFeatureTable) = ValidateDtoTileFeatures phase0.TileFeatures tileTable    
    
    let (raceModifierResults, raceModifierTable) = ValidateDtoRaceModifiers phase0.RaceModifiers    
    
    let (raceResults, raceTable) = ValidateDtoRaces phase0.Races raceModifierTable
    
    let (raceModifierLinkResults, raceModifierLinkTable) = ValidateDtoRaceModifierLinks phase0.RaceModifierLinks raceTable raceModifierTable
    
    let (itemResults, itemTable) = ValidateDtoItems phase0.Items
    
    {
        RawDtoPhase1.Tiles = tileResults
        TileFeatures = tileFeatureResults
        Races = raceResults
        RaceModifiers = raceModifierResults
        RaceModifierLinks = raceModifierLinkResults
        Items = itemResults
    }
    
