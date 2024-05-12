namespace Morgemil.Models

open Morgemil.Models

module Character =
    let DefaultTickActions =
        [ ActionArchetype.CharacterBeforeInput
          ActionArchetype.CharacterEngineInput
          ActionArchetype.CharacterAfterInput ]

    let DefaultPlayerTickActions =
        [ ActionArchetype.CharacterBeforeInput
          ActionArchetype.CharacterPlayerInput
          ActionArchetype.CharacterAfterInput ]
