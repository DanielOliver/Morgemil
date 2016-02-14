namespace Morgemil.Monogame

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

type Window() as this = 
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
  ///Creates a new window and starts it
  static member Start() = 
    let window = new Window()
    window.Run()
