// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
open Morgemil.Map

[<EntryPoint>]
let main argv = 
  printfn "%A" argv
  let seed = 100
  let overW = OverWorld(seed, Morgemil.Math.Vector2i(16, 16), Samples.GrassSample.GrassGenerator)
  let chunk1 = overW.GenerateChunk(Morgemil.Math.Vector2i(0, 0))
  for ny in chunk1.Area.Top..chunk1.Area.Bottom do
    for nx in chunk1.Area.Left..chunk1.Area.Right do
      System.Console.Write(chunk1.Tile(Morgemil.Math.Vector2i(nx, ny)).ID.ToString() + ",")
    System.Console.WriteLine()
  System.Console.ReadKey() |> ignore
  0 // return an integer exit code
