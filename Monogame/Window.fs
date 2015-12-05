namespace Morgemil.Monogame

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

type Window() as this = 
  inherit Game()
  do this.Content.RootDirectory <- "Content"
  let graphics = new GraphicsDeviceManager(this)
  let mutable spriteBatch = Unchecked.defaultof<SpriteBatch>
  
  override x.Initialize() = 
    spriteBatch <- new SpriteBatch(graphics.GraphicsDevice)
    base.Initialize()
    ()
  
  override x.LoadContent() = ()
  override x.Update(gameTime) = ()
  
  override x.Draw(gameTime) = 
    do x.GraphicsDevice.Clear Color.Black
    ()
  
  ///Creates a new window and starts it
  static member Start() = 
    let window = new Window()
    window.Run()
