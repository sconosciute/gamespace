using System;
using System.IO;
using System.Text.Json;
using gamespace.Managers;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace gamespace;

public class Game1 : Game
{
    private const int DefaultResHeight = 800;
    private const int DefaultResWidth = 800;
    private const bool FullScreen = false;
    private const bool DynamicRes = false;
    private const int UpdateTimeDelta = (1000 / 30);
    private double _lastUpTime;
    private readonly GameManager _gm;
    private readonly ILogger _log;

    public Game1()
    {
        
        _log = Globals.LogFactory.CreateLogger<Game1>();
        
        //TODO: Move graphics info to a WindowManager class or include in SettingsManager
        var graphics = new GraphicsDeviceManager(this);
        var adapter = new GraphicsAdapter();
        
        const string fileName = "launchConfig.json";
        var settings = LoadSettings(fileName);
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

        _gm = new GameManager(graphics);
    }

    protected override void Initialize()
    {
        _log.LogInformation("Initializing Game");
        Globals.Init(content: Content, spriteBatch: new SpriteBatch(GraphicsDevice));
        
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _log.LogInformation("Loading textures");
        _gm.AddTexture(Textures.Player);
        _gm.AddTexture(Textures.TestTile);
        _gm.AddTexture(Textures.TestBars);
        _gm.AddTexture(Textures.Collider);
        
        _gm.InitPlayerWorld();
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
    private static LaunchSettings LoadSettings(string fileName)
    {
        var appData = AppDomain.CurrentDomain.BaseDirectory;
        appData = Path.Combine(appData, "Configs");
        var path = Path.Combine(appData, fileName);
        
        Console.Write(path);
        try
        {
            var jsonString = File.ReadAllText(path);
            var settings = JsonSerializer.Deserialize<LaunchSettings>(jsonString);
            return settings;
        }
        catch (FileNotFoundException)
        {
            var defaultSettings = new LaunchSettings
            {
                DefaultResHeight = DefaultResHeight,
                DefaultResWidth = DefaultResWidth,
                IsFullScreened = FullScreen,
                IsDynamic = DynamicRes
            };
            UpdateSettings(path, defaultSettings);
            return defaultSettings;
        }
    }

    private static void UpdateSettings(string path, Object defaultSettings)
    {
        string json = JsonSerializer.Serialize(defaultSettings);
        File.WriteAllText(path, json);
    }
}