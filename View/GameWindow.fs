namespace Morgemil.View

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open SadConsole
open SadConsole.Consoles

type GameWindow() as this = 
  inherit Game()
  do this.Content.RootDirectory <- "Content"
  let graphics = new GraphicsDeviceManager(this)
  let mutable spriteBatch = Unchecked.defaultof<SpriteBatch>
  
  override this.Initialize() = 
    base.Initialize()
    this.Window.Title <- "Morgemil"
    spriteBatch <- new SpriteBatch(graphics.GraphicsDevice)
    this.IsMouseVisible <- true
    this.IsFixedTimeStep <- true
    SadConsole.Engine.Initialize(this.GraphicsDevice)
    SadConsole.Engine.DefaultFont <- SadConsole.Serializer.Deserialize<Font>(System.IO.File.OpenRead("Fonts/IBM.font"))
    SadConsole.Engine.DefaultFont.ResizeGraphicsDeviceManager(graphics, 80, 25, 0, 0)
    let defaultConsole = new Console(80, 25)
    defaultConsole.VirtualCursor.IsVisible <- true
    defaultConsole.CanUseKeyboard <- true
    SadConsole.Engine.ConsoleRenderStack.Add(defaultConsole)
    SadConsole.Engine.ConsoleRenderStack.[0].IsVisible <- true
    SadConsole.Engine.ActiveConsole <- SadConsole.Engine.ConsoleRenderStack.[0]
    base.Initialize()
  
  override this.LoadContent() = ()
  
  override this.Update(gameTime) = 
    SadConsole.Engine.Update(gameTime, this.IsActive)
    base.Update(gameTime)
  
  override this.Draw(gameTime) = 
    this.GraphicsDevice.Clear Color.Black
    SadConsole.Engine.Draw(gameTime)
    base.Draw(gameTime)
  
  static member Start() = 
    let window = new GameWindow()
    window.Run()
