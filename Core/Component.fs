namespace Morgemil.Core

type PositionComponent =
  { EntityId : EntityId
    Position : Vector2i
    Mobile : bool }
  member this.ToComponent() = Component.Position(this)

and PlayerComponent =
  { EntityId : EntityId
    IsHumanControlled : bool }
  member this.ToComponent() = Component.Player(this)

and ResourceComponent =
  { EntityId : EntityId
    Stamina : float<Stamina> }
  member this.ToComponent() = Component.Resource(this)

and ActionComponent =
  { EntityId : EntityId
    TimeOfRequest : float<GameTime>
    TimeOfNextAction : float<GameTime> }
  member this.Duration = this.TimeOfNextAction - this.TimeOfRequest
  member this.ToComponent() = Component.Action(this)

and Component =
  | Position of PositionComponent
  | Player of PlayerComponent
  | Resource of ResourceComponent
  | Action of ActionComponent
  member this.EntityId =
    match this with
    | Component.Position(x) -> x.EntityId
    | Component.Player(x) -> x.EntityId
    | Component.Resource(x) -> x.EntityId
    | Component.Action(x) -> x.EntityId

type ComponentAggregator =
  { EntityId: EntityId
    Position : PositionComponent option
    Player : PlayerComponent option
    Resource : ResourceComponent option
    Action : ActionComponent option
    Triggers : seq<Trigger> }
  static member Empty = { ComponentAggregator.EntityId = EntityId.EntityId(-1)
                          Position = None
                          Player = None
                          Resource = None
                          Action = None
                          Triggers = Seq.empty }

type ComponentChange =
  | Replaced
  | Added
  | Removed
  | Same
  | Unknown

type EntityChanges(oldEntity: ComponentAggregator, newEntity: ComponentAggregator) =
  let change a1 a2 =
    match a1, a2 with
    | Some(x), Some(y) -> if x = y then ComponentChange.Same else ComponentChange.Replaced
    | Some(_), None -> ComponentChange.Removed
    | None, Some(_) -> ComponentChange.Added
    | _ -> ComponentChange.Same
  member this.OldEntity = oldEntity
  member this.NewEntity = newEntity
  member this.Position = change oldEntity.Action newEntity.Action
  member this.Player = change oldEntity.Player newEntity.Player
  member this.Resource = change oldEntity.Resource newEntity.Resource
  member this.Action = change oldEntity.Action newEntity.Action
  member this.Triggers = if oldEntity.Triggers = newEntity.Triggers then ComponentChange.Same else ComponentChange.Unknown