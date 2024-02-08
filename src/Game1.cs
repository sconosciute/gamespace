using System;
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
    
    public static int ScreenHeight;
    public static int ScreenWidth;

    private readonly GraphicsDeviceManager _graphics;
    private Boolean _isFullScreen = false;
    private Boolean _dynamicResolution = false;
    
    private Camera _camera;
    private Player _player;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        //https://industrian.net/tutorials/changing-display-resolution/ CHANGING to correct resolution on current setup
        if (_graphics == null)
        {
            _graphics.ApplyChanges();
        }

        //Creates a new GraphicsAdapter, allowing Game1 to get the resolution of the users monitor, allowing for dynamic resolutions.
        GraphicsAdapter adapter = new GraphicsAdapter();
        if (_dynamicResolution)
        {
            _graphics.PreferredBackBufferWidth = adapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = adapter.CurrentDisplayMode.Height;
        }
        else
        {
            _graphics.PreferredBackBufferWidth = DefaultResolutionWidth;
            _graphics.PreferredBackBufferHeight = DefaultResolutionHeight;
        }

        if (_isFullScreen)
        {
            _graphics.IsFullScreen = _isFullScreen;
        }

        _graphics.ApplyChanges(); //Applies correct resolution and full screens
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        var screenSize = new Vector2(adapter.CurrentDisplayMode.Width, adapter.CurrentDisplayMode.Height);
        Globals.WindowSize = screenSize;
    }

    protected override void Initialize()
    {
        //TODO: Add entity, bgProp, fgProp lists to initialize into.
        //TODO: initialize the player into it's own super special variable.
        
        ScreenHeight = _graphics.PreferredBackBufferHeight; //Gets the height of the screen
        ScreenWidth = _graphics.PreferredBackBufferWidth;   //Gets the width of the screen
        
        World world = new World(200, 200);
        _player = new Player("Player", world);
        
        ScreenWidth = _graphics.PreferredBackBufferWidth; //Gets the width of the screen
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
        _camera.centerOn(_player);
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