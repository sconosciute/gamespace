using System;
using System.IO;
using System.Text.Json;
using gamespace.Managers;
using gamespace.Model;
using gamespace.View;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace gamespace;

public class Game1 : Game
{
    private Camera _camera;
    private Player _player;
    private World _world;
    private const int UpdateTimeDelta = (1000 / 30);
    private double _lastUpTime;

    private RenderObject _playerRender;

    public Game1()
    {
        //TODO: Move graphics info to a WindowManager class or include in SettingsManager
        var graphics = new GraphicsDeviceManager(this);
        var adapter = new GraphicsAdapter();
        
        const string fileName = "launchConfig.json";
        var path = Path.Combine(Environment.CurrentDirectory, "Configs\\", fileName);
        var settings = LoadSettings(path);
        if (settings != null)
        {
            if (settings.IsDynamic)
            {
                graphics.PreferredBackBufferWidth = adapter.CurrentDisplayMode.Width;
                graphics.PreferredBackBufferHeight = adapter.CurrentDisplayMode.Height;
            }
            else
            {
                graphics.PreferredBackBufferWidth = settings.DefaultResWidth;
                graphics.PreferredBackBufferHeight = settings.DefaultResHeight;
            }

            graphics.IsFullScreen = settings.IsFullScreened;
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

        if (_lastUpTime == 0 || now - _lastUpTime >= UpdateTimeDelta)
        {
            _player.FixedUpdate();
            _lastUpTime = now;
        }
        
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        //TODO: Move render code into renderObject, just call draw method on entity and prop lists.
        GraphicsDevice.Clear(Color.CornflowerBlue);
        Globals.SpriteBatch.Begin(transformMatrix: _camera.Translation);
        _world.DebugDrawMap();
        _playerRender.Draw();
        Globals.SpriteBatch.End();
        base.Draw(gameTime);
    }
    private static LaunchSettings LoadSettings(string path)
    {
        try
        {
            var jsonString = File.ReadAllText(path);
            var settings = JsonSerializer.Deserialize<LaunchSettings>(jsonString)!;
            return settings;
        }
        catch (DirectoryNotFoundException)
        {
            return null;
        }
    }
}