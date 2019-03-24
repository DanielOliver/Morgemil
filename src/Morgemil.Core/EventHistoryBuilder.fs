namespace Morgemil.Core

open Morgemil.Models
open Morgemil.Models.Relational

type EventHistoryBuilder<'T, 'U when 'T :> ITableEventHistory<'U> and 'U :> IRow>(table: 'T) =
    member this.Bind(m, f) =
        f m
    member this.Return(x: ActionEvent): 'U Step list =
        let step =
            {   Step.Event = x
                Updates = Table.History table
            }
        Table.ClearHistory table
        [ step ]
    member this.Return(x: 'U Step list ): 'U Step list =
        x
    member this.Zero(): 'U Step list = 
        []
    member this.Yield(x: ActionEvent): 'U Step list =
        let step =
            {   Step.Event = x
                Updates = Table.History table
            }
        Table.ClearHistory table
        [ step ]
    member this.Yield(x: 'U Step list): 'U Step list =
        x
    member this.Combine (a: 'U Step list, b: 'U Step list): 'U Step list =
        List.concat [ b; a ]
    member this.Delay(f) =
        f()

