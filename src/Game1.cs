using gamespace.Managers;
using gamespace.Model;
using gamespace.View;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace gamespace;

public class Game1 : Game
{
    private const int DefaultResolutionWidth = 1280;
    private const int DefaultResolutionHeight = 1024;

    private const bool IsFullScreen = false;
    private const bool DynamicResolution = false;

    private Camera _camera;
    private Player _player;
    private World _world;

    public Game1()
    {
        //TODO: Move graphics info to a WindowManager class or include in SettingsManager
        var graphics = new GraphicsDeviceManager(this);
        GraphicsAdapter adapter = new GraphicsAdapter();

        if (DynamicResolution)
        {
            graphics.PreferredBackBufferWidth = adapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = adapter.CurrentDisplayMode.Height;
        }
        else
        {
            graphics.PreferredBackBufferWidth = DefaultResolutionWidth;
            graphics.PreferredBackBufferHeight = DefaultResolutionHeight;
        }

        if (IsFullScreen)
        {
            graphics.IsFullScreen = IsFullScreen;
        }

        graphics.ApplyChanges();
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        var screenSize = new Vector2(adapter.CurrentDisplayMode.Width, adapter.CurrentDisplayMode.Height);
        Globals.WindowSize = screenSize;
    }

    protected override void Initialize()
    {
        //TODO: Add entity, bgProp, fgProp lists to initialize into or determine to add to world lists.
        World world = new World(50, 50);
        _player = new Player("Player", world);
        base.Initialize();
    }

    protected override void LoadContent()
    {
        //TODO: Load content inside render object.
        _camera = new Camera(_player.EntityId);
        _player.EntityEvent += _camera.HandleEntityEvent;
    }

    protected override void Update(GameTime gameTime)
    {
        //TODO: Make work with fixed update step
        InputManager.Update();
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        //TODO: Move render code into renderObject, just call draw method on entity and prop lists.
        GraphicsDevice.Clear(Color.CornflowerBlue);
        Globals.SpriteBatch.Begin(transformMatrix: _camera.Translation);

        base.Draw(gameTime);
    }
}