namespace Morgemil.Monogame

type ElementProperties = 
  { Area : ScreenRectangle
    Visible : bool
    Name : string
    Text : string }
  
  static member Button = 
    { Area = ScreenRectangle.FullScreen
      Visible = true
      Name = ""
      Text = "" }
  
  static member Textbox = 
    { Area = ScreenRectangle.FullScreen
      Visible = true
      Name = ""
      Text = "" }
  
  static member Panel = 
    { Area = ScreenRectangle.FullScreen
      Visible = false
      Name = ""
      Text = "" }

type ElementType<'a> = 
  | Textbox of Properties : ('a -> ElementProperties) * Id : int
  | Button of Properties : ('a -> ElementProperties) * Id : int
  | Panel of Children : ElementType<'a> list * Properties : ('a -> ElementProperties) * Id : int
  
  member this.Id = 
    match this with
    | Textbox(_, id) -> id
    | Button(_, id) -> id
    | Panel(_, _, id) -> id
  
  member this.Name = 
    match this with
    | Textbox(_, id) -> "Textbox " + id.ToString()
    | Button(_, id) -> "Button " + id.ToString()
    | Panel(_, _, id) -> "Panel " + id.ToString()
  
  member this.Setters = 
    match this with
    | Textbox(x, id) -> [ (id, x) ]
    | Button(x, id) -> [ (id, x) ]
    | Panel(children, x, id) -> 
      (id, x) :: (children
                  |> List.map (fun t -> t.Setters)
                  |> List.concat)
  
  member this.Children = 
    match this with
    | Panel(x, _, _) -> x
    | _ -> []
