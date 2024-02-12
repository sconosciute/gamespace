using System;
using System.IO;
using System.Text.Json;
using gamespace.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace gamespace;

public class Game1 : Game
{
    private const int UpdateTimeDelta = (1000 / 30);
    private double _lastUpTime;
    private readonly GameManager _gm;


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

        _gm = new GameManager(this, graphics);
    }

    protected override void Initialize()
    {
        InitGlobals();
        
        base.Initialize();
    }

    private void InitGlobals()
    {
        Globals.SpriteBatch = new SpriteBatch(GraphicsDevice);
        Globals.Content = Content;
    }

    protected override void LoadContent()
    {
        _gm.AddTexture(Content.Load<Texture2D>(Textures.Player));
        _gm.AddTexture(Content.Load<Texture2D>(Textures.TestTile));
        _gm.AddTexture(Content.Load<Texture2D>(Textures.TestBars));
        
        _gm.InitPlayerRender();
    }

    protected override void Update(GameTime gameTime)
    {
        var now = gameTime.TotalGameTime.TotalMilliseconds;
        InputManager.Update();

        if (_lastUpTime == 0 || now - _lastUpTime >= UpdateTimeDelta)
        {
            _gm.FixedUpdate();
            _lastUpTime = now;
        }
        
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        _gm.Draw();
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