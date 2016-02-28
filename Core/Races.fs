module Morgemil.Core.Races

let Human = 
  { Id = 0
    Noun = "Human"
    Adjective = "Human"
    Description = 
      "A decimated but proud race, these stout allies excel at physical activities but lack the mystical attunement for true sorcery." }

let Pixie = 
  { Id = 1
    Noun = "Pixie"
    Adjective = "Pixie"
    Description = "These agile faerie creatures are magically inclined but avoid confrontation." }

let Items = [| Human; Pixie |]

let Lookup = 
  Items
  |> Seq.map (fun t -> (t.Id, t))
  |> Map.ofSeq
