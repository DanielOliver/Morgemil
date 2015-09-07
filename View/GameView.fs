namespace Morgemil.View

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input
open Morgemil.Game
open Morgemil.Map
open Morgemil.Test

type GameView() as this = 
  inherit Game()
  
  let level = 
    DungeonGeneration.Generate { Type = DungeonGenerationType.Square
                                 Depth = 1
                                 RngSeed = 656556 }
  
  //Assumes there is at least one entrance. Takes the first one
  let entrance = 
    (TileModifier.Entrance(Morgemil.Math.Rectangle(Morgemil.Math.Vector2i(5, 5), Morgemil.Math.Vector2i(1))) 
     :: level.TileModifiers)
    |> List.choose (function 
         | TileModifier.Entrance(location) -> Some(location)
         | _ -> None)
    |> List.rev
    |> List.head
  
  //Replace the player
  let mutable walkAbout = 
    Walkabout(level, 
              { Id = 5
                Race = Race.Lookup.[0]
                Position = entrance.Position })
  
  let graphics = new GraphicsDeviceManager(this)
  do graphics.PreferredBackBufferWidth <- 640
  do graphics.PreferredBackBufferHeight <- 480
  do graphics.ApplyChanges()
  let mutable spriteBatch = Unchecked.defaultof<SpriteBatch> //null
  let mutable spriteTexture = Unchecked.defaultof<Texture2D> //null
  let mutable camera = Camera2d.Default graphics level
  let mutable hasGraphicsChanges = false
  
  ///Chooses a color for a map tile 
  let ChooseColor(tileDef : Morgemil.Map.Tile) = 
    match tileDef.BlocksMovement with
    | true -> Color.Red
    | false -> Color.White
  
  ///Draws a map tile
  let DrawTile(pos : Morgemil.Math.Vector2i, tileDef : Morgemil.Map.Tile) = 
    let drawArea = Rectangle(pos.X, pos.Y, 1, 1)
    spriteBatch.Draw(spriteTexture, drawArea, ChooseColor tileDef)
  
  let mutable firstFire = true
  let mutable lastKey = Keys.Space
  
  ///Zooms out to show the map
  member private this.ShowMap() = camera <- camera.ShowMap()
  
  ///Resizes the graphics buffer to match window resolution
  member private this.ResizeWindow() = 
    graphics.PreferredBackBufferWidth <- base.Window.ClientBounds.Width
    graphics.PreferredBackBufferHeight <- base.Window.ClientBounds.Height
    hasGraphicsChanges <- true
    this.ShowMap()
  
  override this.Initialize() = 
    base.Initialize()
    base.Window.Title <- "Morgemil"
    base.Window.AllowUserResizing <- true
    base.Window.ClientSizeChanged.Add(fun evArgs -> this.ResizeWindow())
    base.IsMouseVisible <- true
    this.ShowMap()
  
  override this.LoadContent() = 
    spriteBatch <- new SpriteBatch(this.GraphicsDevice)
    spriteTexture <- new Texture2D(this.GraphicsDevice, 1, 1) //Only need 1 color
    spriteTexture.SetData([| Color.White |]) //255,255,255
  
  override this.Update(gameTime) = 
    let state = Keyboard.GetState()
    if hasGraphicsChanges then 
      graphics.ApplyChanges()
      hasGraphicsChanges <- false
    if state.IsKeyDown(Keys.Left) && firstFire then 
      walkAbout <- walkAbout.Act(Actions.MoveWest)
      lastKey <- Keys.Left
      firstFire <- false
    if state.IsKeyDown(Keys.Right) && firstFire then 
      walkAbout <- walkAbout.Act(Actions.MoveEast)
      lastKey <- Keys.Right
      firstFire <- false
    if state.IsKeyDown(Keys.Up) && firstFire then 
      walkAbout <- walkAbout.Act(Actions.MoveSouth)
      lastKey <- Keys.Up
      firstFire <- false
    if state.IsKeyDown(Keys.Down) && firstFire then 
      walkAbout <- walkAbout.Act(Actions.MoveNorth)
      lastKey <- Keys.Down
      firstFire <- false
    if (state.IsKeyUp(lastKey)) then 
      System.Diagnostics.Debug.WriteLine(walkAbout.Player.Position)
      firstFire <- true
    camera <- camera.CenterCamera(64.0f,walkAbout.Player.Area.Position)
    ()
  
  override this.Draw(gameTime) = 
    this.GraphicsDevice.Clear Color.Black
    //Adjust the camera by the buffer size
    let transform = camera.Matrix
    spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, new System.Nullable<Matrix>(transform))
    level.TileCoordinates |> Seq.iter (DrawTile)
    let mouse_state = Mouse.GetState().Position.ToVector2()
    let world = camera.ScreenCoordinatesToWorld(mouse_state)
    let drawArea = Rectangle(world.X, world.Y, 1, 1)
    spriteBatch.Draw(spriteTexture, drawArea, Color.Green)
    spriteBatch.Draw
      (spriteTexture, Rectangle(walkAbout.Player.Position.X, walkAbout.Player.Position.Y, 1, 1), Color.Blue)
    spriteBatch.End()
