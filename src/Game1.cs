using System;
using System.Xml.Schema;
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
    //Player control Keys
    private const Keys ForwardButton = Keys.W;
    private const Keys LeftButton = Keys.A;
    private const Keys RightButton = Keys.D;
    private const Keys DownButton = Keys.S;
    private const Keys SprintButton = Keys.Space;


    public Animation AnimationTimer; 
    
    public static int ScreenHeight;
    public static int ScreenWidth;

    private readonly GraphicsDeviceManager _graphics;
    private Boolean _isFullScreen = false;
    private Boolean _dynamicResolution = false;

    private SpriteBatch _spriteBatch;
    private Camera _camera;
    private Player _player;
    private Texture2D _playerModel;
    private Texture2D _testingBackGround;
    private Texture2D _spaceBackground;
    
    private RenderObject _renderPlayer;
    private RenderObject _renderTempBackground;
    private RenderObject _renderSpaceBackground;

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
        ScreenHeight = _graphics.PreferredBackBufferHeight; //Gets the height of the screen
        ScreenWidth = _graphics.PreferredBackBufferWidth;   //Gets the width of the screen
        AnimationTimer = new Animation();
        _player = new Player("Player32", "Rogue", 1, _renderPlayer, 0, 0, 32, 32, true,
            true, 100, 100, 100, 100, 10);
        
        ScreenWidth = _graphics.PreferredBackBufferWidth; //Gets the width of the screen
        base.Initialize();
    }

    protected override void LoadContent()
    {
        //TODO: move this into renderObject.
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _testingBackGround = Content.Load<Texture2D>("checkeredBoardBackGround");
        _playerModel = Content.Load<Texture2D>("playerCircle32");
        _spaceBackground = Content.Load<Texture2D>("spaceBackgroundResized");
        
        _renderPlayer = new RenderObject(_playerModel, _player.ReturnPos(), 1, 1); //Have to load here to avoid _playerModel being NULL
        _renderTempBackground = new RenderObject(_testingBackGround, Vector2.Zero, 1, 1); //Same problem
        _renderSpaceBackground = new RenderObject(_spaceBackground, Vector2.Zero, 1, 1);
        
        _camera = new Camera();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        
        PlayerInput(gameTime);
        
        _player.Update(gameTime); 
        _renderPlayer.Position = _player.ReturnPos();
        AnimationTimer.Update(gameTime);
        _camera.centerOn(_player); //Calls this after every update to keep player centered
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        //TODO: Move render code into renderObject, just call draw method on entity and prop lists.
        GraphicsDevice.Clear(Color.CornflowerBlue);
        
        _spriteBatch.Begin(transformMatrix: _camera.Transform);
        Vector2 playerSize = Vector2.One;
        _spriteBatch.Draw(_testingBackGround, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, Vector2.One,
            SpriteEffects.None, 1f);
        _spriteBatch.Draw(_playerModel, _player.ReturnPos(), null, Color.White, 0f, Vector2.Zero, playerSize,
            SpriteEffects.None, 0f); //Layer 0f background, 1f foreground
        _renderTempBackground.Draw(_spriteBatch);
        _renderSpaceBackground.Draw(_spriteBatch);
        _renderPlayer.Draw(_spriteBatch);
        _spriteBatch.End();
        base.Draw(gameTime);
    }

    private void PlayerInput(GameTime gameTime)
    {
        //TODO: Rework system to fire events that Player listens for to move.
    }
}