namespace Morgemil.View

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input

type GameView() as this = 
  inherit Game()
  
  let level = 
    Morgemil.Map.DungeonGeneration.Generate { Type = Morgemil.Map.DungeonGenerationType.Square
                                              Depth = 1
                                              RngSeed = 6456 }
  
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
    if state.IsKeyDown(Keys.Space) then 
      if firstFire then 
        let mouse_state = Mouse.GetState().Position.ToVector2()
        System.Diagnostics.Debug.WriteLine(camera.ScreenCoordinatesToWorld(mouse_state))
      firstFire <- false
    if state.IsKeyUp(Keys.Space) then firstFire <- true
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
    spriteBatch.End()
