using System;
using System.Xml.Schema;
using gamespace.Managers;
using gamespace.Model;
using gamespace.View;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace gamespace;

public class Game1 : Game
{
    //Use these constants and set _dynamicResolution to false in order to have a static width and height
    //  Looking at implementing a JSON file to contain these system settings
    private const int DefaultResolutionWidth = 1280;
    private const int DefaultResolutionHeight = 1024;


    public Animation AnimationTimer; 
    
    public static int ScreenHeight;
    public static int ScreenWidth;

    private readonly GraphicsDeviceManager _graphics;
    private Boolean _isFullScreen = false;
    private Boolean _dynamicResolution = false;

    private SpriteBatch _spriteBatch;
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
    }

    protected override void Initialize()
    {
        //TODO: Add entity, bgProp, fgProp lists to initialize into.
        //TODO: initialize the player into it's own super special variable.
        World world = new World(200, 200);
        ScreenHeight = _graphics.PreferredBackBufferHeight; //Gets the height of the screen
        ScreenWidth = _graphics.PreferredBackBufferWidth;   //Gets the width of the screen
        AnimationTimer = new Animation();
        _player = new Player("Player", world);
        
        ScreenWidth = _graphics.PreferredBackBufferWidth; //Gets the width of the screen
        base.Initialize();
    }

    protected override void LoadContent()
    {
        //TODO: Load content inside render object.
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        
        _camera = new Camera();
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
        
        base.Draw(gameTime);
    }
}