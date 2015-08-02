namespace Morgemil.View

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input

type GameView() as this = 
  inherit Game()
  
  let level = 
    Morgemil.Map.DungeonGeneration.Generate { Type = Morgemil.Map.DungeonGenerationType.Square
                                              Depth = 1
                                              RngSeed = 656556 }
  
  let graphics = new GraphicsDeviceManager(this)
  do graphics.PreferredBackBufferWidth <- 1024
  do graphics.PreferredBackBufferHeight <- 768
  do graphics.ApplyChanges()
  let mutable spriteBatch = Unchecked.defaultof<SpriteBatch> //null
  let mutable spriteTexture = Unchecked.defaultof<Texture2D> //null
  
  ///Returns a centered camera. Given the tile location
  let CenterCamera zoom (tileLocation : Morgemil.Math.Vector2i) = 
    Camera2d(zoom, Vector2(float32 (-tileLocation.X), float32 (-tileLocation.Y)))
  
  let mutable camera = CenterCamera 8.0f (Morgemil.Math.Vector2i(level.Area.Width / 2, level.Area.Height / 2))
  
  let ChooseColor(tileDef : Morgemil.Map.TileDefinition) = 
    match tileDef.BlocksMovement with
    | true -> Color.Red
    | false -> Color.White
  
  let DrawTile(pos : Morgemil.Math.Vector2i, tileDef : Morgemil.Map.TileDefinition) = 
    let drawArea = Rectangle(pos.X, pos.Y, 1, 1)
    spriteBatch.Draw(spriteTexture, drawArea, ChooseColor tileDef)
  
  override this.Initialize() = 
    base.Initialize()
    base.Window.Title <- "Morgemil"
  
  override this.LoadContent() = 
    spriteBatch <- new SpriteBatch(this.GraphicsDevice)
    spriteTexture <- new Texture2D(this.GraphicsDevice, 1, 1) //Only need 1 color
    spriteTexture.SetData([| Color.White |]) //255,255,255
  
  override this.Update(gameTime) = 
    let state = Keyboard.GetState()
    if state.IsKeyDown(Keys.A) then camera <- camera.AddZoom(1.0f)
  
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
