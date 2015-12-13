module Morgemil.Monogame.Gui

///Create the gui from an ID generation function
let create (gui : (unit -> int) -> ElementType<'a>) (x) = 
  let root = gui x
  let setters = root.Setters |> Map.ofList
  
  let set (vm : 'a) = 
    root.Setters
    |> List.map (fun (id, prop) -> id, prop (vm))
    |> Map.ofList
  (root, setters, set)

///Create a readonly textbox
let textbox vm = (fun x -> ElementType.Textbox(vm, x()))

///Create a button
let button vm = (fun x -> ElementType.Button(vm, x()))

///Create a panel to hold other controls
let panel vm (children : ((unit -> int) -> ElementType<_>) list) = 
  (fun x -> ElementType.Panel(children |> List.map (fun t -> t (x)), vm, x()))
