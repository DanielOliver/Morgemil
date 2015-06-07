namespace Morgemil.View

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

type GameView() as this =
  inherit Game()
  let chunk = Morgemil.Map.DungeonGeneration.Generate 216798
  let tileSize = 2
  let graphics = new GraphicsDeviceManager(this)
  do graphics.PreferredBackBufferWidth <- chunk.Area.Width * tileSize
  do graphics.PreferredBackBufferHeight <- chunk.Area.Height * tileSize
  do graphics.ApplyChanges()
  let mutable spriteBatch = Unchecked.defaultof<SpriteBatch> //null
  let mutable spriteTexture = Unchecked.defaultof<Texture2D> //null

  let ChooseColor(tileDef : Morgemil.Map.TileDefinition) =
    match tileDef.BlocksMovement with
    | true -> Color.Black
    | false -> Color.White

  let DrawTile(pos : Morgemil.Math.Vector2i, tileDef : Morgemil.Map.TileDefinition) =
    let drawArea = Rectangle(pos.X * tileSize, pos.Y * tileSize, tileSize, tileSize)
    spriteBatch.Draw(spriteTexture, drawArea, ChooseColor tileDef)

  override this.Initialize() =
    base.Initialize()
    base.Window.Title <- "Morgemil"

  override this.LoadContent() =
    spriteBatch <- new SpriteBatch(this.GraphicsDevice)
    spriteTexture <- new Texture2D(this.GraphicsDevice, 1, 1) //Only need 1 color
    spriteTexture.SetData([| Color.White |]) //255,255,255

  override this.Update(gameTime) = ()
  override this.Draw(gameTime) =
    this.GraphicsDevice.Clear Color.Black
    spriteBatch.Begin()
    chunk.TileCoordinates |> Seq.iter (DrawTile)
    spriteBatch.End()
