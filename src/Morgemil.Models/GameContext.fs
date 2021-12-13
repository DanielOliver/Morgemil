namespace Morgemil.Models

open Morgemil.Models

[<Record>]
type GameContext =
    {
        CurrentTimeTick: int64<TimeTick>
    }

