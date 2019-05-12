namespace Morgemil.Core

open Morgemil.Models
open Morgemil.Models.Relational

type TileFeatureTable(tileFeatures: TileFeature seq) =
    inherit ReadonlyTable<TileFeature, TileFeatureID>(tileFeatures, fun t -> t.Key)

    member this.GetFeaturesForTile(tileID: TileID): TileFeature seq =
        tileFeatures
        |> Seq.where( fun t -> t.PossibleTiles |> List.map(fun t -> t.ID) |> List.contains tileID)

module TileFeatureTable =
    
    let GetFeaturesForTile(tileID: TileID) (table: IReadonlyTable<TileFeature, TileFeatureID>): TileFeature seq =
        table.Items
        |> Seq.where( fun t -> t.PossibleTiles |> List.map(fun t -> t.ID) |> List.contains tileID)
        
