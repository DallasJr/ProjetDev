using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceWar;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Screen currentScreen;

    public SpriteFont TextFont { get; private set; }
    public SpriteFont TitleFont { get; private set; }
    public SpriteFont MidFont { get; private set; }

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
        IsMouseVisible = true;
        _graphics.IsFullScreen = false;
    }

    protected override void Initialize()
    {
        currentScreen = new MenuScreen(this);
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        TitleFont = Content.Load<SpriteFont>("TitleFont");
        MidFont = Content.Load<SpriteFont>("MidFont");
        TextFont = Content.Load<SpriteFont>("TextFont");
        currentScreen.LoadContent(Content);

        currentScreen.LoadContent(Content);
    }

    protected override void Update(GameTime gameTime)
    {
        currentScreen.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        _spriteBatch.Begin();
        currentScreen.Draw(_spriteBatch);
        _spriteBatch.End();
        base.Draw(gameTime);
    }

    public void ChangeScreen(Screen newScreen) {
        newScreen.LoadContent(Content);
        currentScreen = newScreen;
    }
}
