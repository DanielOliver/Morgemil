module Program

open Morgemil.Monogame
open Morgemil.Monogame.Gui

type Wasd() = 
  member this.Yes = 1

let view = 
  panel (fun (t : Wasd) -> ElementProperties.Panel) 
    [ textbox (fun _ -> ElementProperties.Textbox)
      panel (fun _ -> ElementProperties.Panel) [ textbox (fun _ -> ElementProperties.Textbox)
                                                 button (fun _ -> ElementProperties.Button)
                                                 panel (fun _ -> ElementProperties.Panel) [] ]
      button (fun _ -> ElementProperties.Button)
      panel (fun _ -> ElementProperties.Panel) [ button (fun _ -> ElementProperties.Button)
                                                 textbox (fun _ -> ElementProperties.Textbox) ] ]

let pool = Morgemil.Logic.IdentityPool(Set.empty, id, id)
let five = create view (pool.Generate)

[<EntryPoint>]
let main argv = 
  Window.Start()
  let (gui, setters, set) = five
  
  let rec draw (depth : int) (element : ElementType<Wasd>) = 
    printfn "%s%s" ("".PadLeft(depth)) element.Name
    for i in element.Children do
      draw (depth + 4) i
  draw 0 gui
  let wasd = Wasd()
  let props = set wasd
  printfn "%A" props
  System.Console.ReadKey() |> ignore
  0
