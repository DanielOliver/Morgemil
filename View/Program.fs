[<EntryPoint>]
let main argv =
  let game = new Morgemil.View.GameView()
  game.Run()
  0 // return an integer exit code
