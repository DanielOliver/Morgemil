namespace Morgemil.Logic

open Morgemil.Core

module Message = 
  let ResourceChange(oldValue : ResourceComponent, newValue : ResourceComponent) = 
    EventResult.EntityResourceChanged { EntityId = oldValue.EntityId
                                        OldValue = oldValue
                                        NewValue = newValue }
  
  let PositionChange(oldValue : PositionComponent, newValue : PositionComponent) = 
    EventResult.EntityMoved { EntityId = oldValue.EntityId
                              MovedFrom = oldValue.Position
                              MovedTo = newValue.Position }
