using System;
using System.Collections.Generic;
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
    private int _updateTimeDelta = (1000 / 30);
    private double _lastUpTime;
    private double _frameTimeAccumulator;

    private RenderObject _playerRender;

    private List<RenderObject> _renderObjects;

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
    }

    protected override void Initialize()
    {
        //TODO: Add entity, bgProp, fgProp lists to initialize into or determine to add to world lists.
        InitGlobals();
        _world = new World(51, 51);
        _player = new Player("Player", _world);
        var playerTexture = Content.Load<Texture2D>("playerCircle32");
        _playerRender = new RenderObject(playerTexture, _player.WorldCoordinate, _player.EntityId);
        _player.EntityEvent += _playerRender.HandleEntityEvent;
        _renderObjects = new List<RenderObject>();
        
        base.Initialize();
    }

    private void InitGlobals()
    {
        Globals.SpriteBatch = new SpriteBatch(GraphicsDevice);
        Globals.Content = Content;
        Globals.WindowSize = new Vector2(1280, 1024);
    }

    protected override void LoadContent()
    {
        //TODO: Load content inside render object.
        _camera = new Camera(_player.EntityId);
        _player.EntityEvent += _camera.HandleEntityEvent;
    }

    protected override void Update(GameTime gameTime)
    {
        var now = gameTime.TotalGameTime.TotalMilliseconds;
        InputManager.Update();

        if (_lastUpTime == 0 || now - _lastUpTime >= _updateTimeDelta)
        {
            _player.FixedUpdate();
        }
        
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        //TODO: Move render code into renderObject, just call draw method on entity and prop lists.
        GraphicsDevice.Clear(Color.CornflowerBlue);
        Globals.SpriteBatch.Begin(transformMatrix: _camera.Translation);
        _world.DebugDrawMap();
        foreach (RenderObject renderObject in _renderObjects)
        {
            renderObject.Draw();
        }
        _playerRender.Draw();
        Globals.SpriteBatch.End();
        base.Draw(gameTime);
    }
}