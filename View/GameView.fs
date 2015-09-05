﻿namespace Morgemil.View

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
  let mutable camera = Camera2d.Default
  let mutable hasGraphicsChanges = false
  
  let ChooseColor(tileDef : Morgemil.Map.Tile) = 
    match tileDef.BlocksMovement with
    | true -> Color.Red
    | false -> Color.White
  
  let DrawTile(pos : Morgemil.Math.Vector2i, tileDef : Morgemil.Map.Tile) = 
    let drawArea = Rectangle(pos.X, pos.Y, 1, 1)
    spriteBatch.Draw(spriteTexture, drawArea, ChooseColor tileDef)
  
  member private this.ResizeWindow() = 
    graphics.PreferredBackBufferWidth <- base.Window.ClientBounds.Width
    graphics.PreferredBackBufferHeight <- base.Window.ClientBounds.Height
    camera <- Camera2d.ShowMap level 
                (Morgemil.Math.Vector2i(base.Window.ClientBounds.Width, base.Window.ClientBounds.Height))
    hasGraphicsChanges <- true
  
  override this.Initialize() = 
    base.Initialize()
    base.Window.Title <- "Morgemil"
    base.Window.AllowUserResizing <- true
    base.Window.ClientSizeChanged.Add(fun evArgs -> this.ResizeWindow())
    camera <- Camera2d.ShowMap level 
                (Morgemil.Math.Vector2i(base.Window.ClientBounds.Width, base.Window.ClientBounds.Height))
  
  override this.LoadContent() = 
    spriteBatch <- new SpriteBatch(this.GraphicsDevice)
    spriteTexture <- new Texture2D(this.GraphicsDevice, 1, 1) //Only need 1 color
    spriteTexture.SetData([| Color.White |]) //255,255,255
  
  override this.Update(gameTime) = 
    let state = Keyboard.GetState()
    if hasGraphicsChanges then 
      graphics.ApplyChanges()
      hasGraphicsChanges <- false
    ()
  
  //    if state.IsKeyDown(Keys.A) then camera <- camera.AddZoom(1.0f)
  //    if state.IsKeyDown(Keys.D) then camera <- camera.SetZoom(camera.Zoom * 0.95f)
  override this.Draw(gameTime) = 
    this.GraphicsDevice.Clear Color.Black
    //Adjust the camera by the buffer size
    let transform = 
      camera.Matrix 
      * Matrix.CreateTranslation
          (float32 (graphics.PreferredBackBufferWidth / 2), float32 (graphics.PreferredBackBufferHeight / 2), 0.0f)
    spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, new System.Nullable<Matrix>(transform))
    level.TileCoordinates |> Seq.iter (DrawTile)
    spriteBatch.End()
