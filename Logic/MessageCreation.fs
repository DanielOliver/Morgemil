namespace Morgemil.Logic

open Morgemil.Core

module Message = 
  let ResourceChange (old_value : ResourceComponent,new_value : ResourceComponent) = 
    EventResult.EntityResourceChanged { EntityId = old_value.EntityId
                                        OldValue = old_value.ResourceAmount
                                        NewValue = new_value.ResourceAmount }
  
  let PositionChange (old_value : PositionComponent,new_value : PositionComponent) = 
    EventResult.EntityMoved { EntityId = old_value.EntityId
                              MovedFrom = old_value.Position
                              MovedTo = new_value.Position }
