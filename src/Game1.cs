﻿using System;
using System.IO;
using System.Text.Json;
using gamespace.Managers;
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

    private RenderObject _playerRender;

    public Game1()
    {
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
    private static LaunchSettings LoadSettings(string fileName)
    {
        try
        {
            //var path = Path.Combine(Environment.CurrentDirectory, "Configs\\", fileName);
            var jsonString = File.ReadAllText(fileName);
            var settings = JsonSerializer.Deserialize<LaunchSettings>(jsonString);
            return settings;
        }
        catch (DirectoryNotFoundException)
        {
            LaunchSettings defaultSettings = new LaunchSettings();
            defaultSettings.DefaultResHeight = DefaultResHeight;
            defaultSettings.DefaultResWidth = DefaultResWidth;
            defaultSettings.IsFullScreened = FullScreen;
            defaultSettings.IsDynamic = DynamicRes;
            UpdateSettings(fileName, defaultSettings);
            return defaultSettings;
        }
    }

    private static void UpdateSettings(string path, Object defaultSettings)
    {
        string json = JsonSerializer.Serialize(defaultSettings);
        File.WriteAllText(path, json);
    }
}