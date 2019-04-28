namespace Morgemil.Core

open Morgemil.Models

type TileFeatureTable(tileFeatures: TileFeature seq) =
    inherit ReadonlyTable<TileFeature, TileFeatureID>(tileFeatures, fun t -> t.Key)

    member this.GetFeaturesForTile(tileID: TileID): TileFeature seq =
        tileFeatures
        |> Seq.where( fun t -> t.PossibleTiles |> List.map(fun t -> t.ID) |> List.contains tileID)
