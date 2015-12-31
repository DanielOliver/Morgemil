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
    this.Window.AllowUserResizing <- true
    spriteBatch <- new SpriteBatch(graphics.GraphicsDevice)
    this.IsMouseVisible <- true
    this.IsFixedTimeStep <- true
    SadConsole.Engine.Initialize(this.GraphicsDevice)
    SadConsole.Engine.DefaultFont <- SadConsole.Serializer.Deserialize<Font>
                                       (System.IO.File.OpenRead("Fonts/Cheepicus12.font"))
    let defaultConsole = SplashConsole()
    defaultConsole.ResizeGraphicsDeviceManager(graphics, 0, 0)
    defaultConsole.VirtualCursor.IsVisible <- true
    defaultConsole.CanUseKeyboard <- true
    SadConsole.Engine.ConsoleRenderStack.Add(defaultConsole)
    SadConsole.Engine.ConsoleRenderStack.[0].IsVisible <- true
    SadConsole.Engine.ActiveConsole <- SadConsole.Engine.ConsoleRenderStack.[0]
    this.Window.ClientSizeChanged.Add(fun _ ->
      if graphics.PreferredBackBufferWidth <> this.Window.ClientBounds.Width
         || graphics.PreferredBackBufferHeight <> this.Window.ClientBounds.Height then
        let activeConsole = SadConsole.Engine.ActiveConsole
        let width = this.Window.ClientBounds.Width / SadConsole.Engine.DefaultFont.CellWidth
        let widthRemain = this.Window.ClientBounds.Width % SadConsole.Engine.DefaultFont.CellWidth
        let height = this.Window.ClientBounds.Height / SadConsole.Engine.DefaultFont.CellHeight
        let heightRemain = this.Window.ClientBounds.Height % SadConsole.Engine.DefaultFont.CellHeight
        if height <> 0 && width <> 0 then
          activeConsole.CellData.Resize(width, height)
          SadConsole.Engine.DefaultFont.ResizeGraphicsDeviceManager(graphics, width, height, widthRemain, heightRemain))
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