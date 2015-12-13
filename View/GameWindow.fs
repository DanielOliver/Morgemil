namespace Morgemil.View

open System
open SadConsole
open SadConsole.Consoles
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

type GameWindow() as this =
  inherit Game()
  do this.Content.RootDirectory <- "Content"
  let graphics = new GraphicsDeviceManager(this)
  let mutable spriteBatch = Unchecked.defaultof<SpriteBatch>
  
  override this.Initialize() = 
    base.Initialize()
    this.Window.Title <- "Morgemil"
    spriteBatch <- new SpriteBatch(graphics.GraphicsDevice)
  
  override this.LoadContent() = ()
  override this.Update(gameTime) = ()
  override this.Draw(gameTime) = this.GraphicsDevice.Clear Color.Black
  static member Start() = 
    let window = new GameWindow()
    window.Run()
