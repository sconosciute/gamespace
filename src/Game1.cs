using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;
using gamespace.Managers;
using gamespace.Model;
using gamespace.View;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace gamespace;

public class Game1 : Game
{
    private int DefaultResolutionWidth;
    private int DefaultResolutionHeight;

    private bool IsFullScreen;
    private bool DynamicResolution;

    private Camera _camera;
    private Player _player;
    private World _world;

    private RenderObject _playerRender;

    private List<RenderObject> _renderObjects;

    public Game1()
    {
        //TODO: Move graphics info to a WindowManager class or include in SettingsManager
        var graphics = new GraphicsDeviceManager(this);
        GraphicsAdapter adapter = new GraphicsAdapter();
        
        String fileName = "launchConfig.json";
        string path = Path.Combine(Environment.CurrentDirectory, "Configs\\", fileName);
        launchSettings? settings = loadSettings(path);
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
        //Console.Write(settings.IsDynamic);
        //Console.Write(settings.IsFullScreened);
        //Console.Write(settings.DefaultResWidth);
        //Console.WriteLine($"Date: {settings?.DefaultResWidth}");
        //Console.Write(settings.DefaultResHeight);
        Console.Write(Path.Combine(AppDomain.CurrentDomain.BaseDirectory + path));
        graphics.ApplyChanges();
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        var screenSize = new Vector2(adapter.CurrentDisplayMode.Width, adapter.CurrentDisplayMode.Height);
        Globals.WindowSize = screenSize;
    }

    protected override void Initialize()
    {
        //TODO: Add entity, bgProp, fgProp lists to initialize into or determine to add to world lists.
        Globals.SpriteBatch = new SpriteBatch(GraphicsDevice);
        World world = new World(51, 51);
        _player = new Player("Player", world);
        var tileTexture = Content.Load<Texture2D>("tile");
        var playerTexture = Content.Load<Texture2D>("playerCircle32");
        var dummy = new Guid();
        _playerRender = new RenderObject(playerTexture, _player.WorldCoordinate, _player.EntityId);
        _renderObjects = new List<RenderObject>();
        
        for (int x = -25; x <= 25; x++)
        {
            for (int y = -25; y <= 25; y++)
            {
                var position = new Vector2(x, y);
                var prop = new Prop(position, 1, 1);
                Tile tile = new Tile(prop);
                _renderObjects.Add(new RenderObject(tileTexture, position, dummy));
            }
        }
        
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
        Globals.SpriteBatch.Begin();
        foreach (RenderObject renderObject in _renderObjects)
        {
            renderObject.Draw();
        }
        _playerRender.Draw();
        Globals.SpriteBatch.End();
        base.Draw(gameTime);
    }
    private launchSettings loadSettings(string path)
    {
        string jsonString = File.ReadAllText(path);
        launchSettings settings = JsonSerializer.Deserialize<launchSettings>(jsonString)!;
        return settings;
    }
}