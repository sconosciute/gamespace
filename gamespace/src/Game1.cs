﻿using gamespace.Managers;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace gamespace;

public class Game1 : Game
{
    private const int UpdateTimeDelta = (1000 / 30);
    private double _lastUpTime;
    private readonly GameManager _gm;
    private readonly ILogger _log;
    private GuiManager _gui;

    public SpriteFont Font;

    public Game1()
    {
        
        _log = Globals.LogFactory.CreateLogger<Game1>();
        
        //TODO: Move graphics info to a WindowManager class or include in SettingsManager
        var graphics = SettingsManager.GenerateGraphics(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        _gm = new GameManager(graphics);
        _gui = _gm.InitGui();
    }

    protected override void Initialize()
    {
        _log.LogInformation("Initializing Game");
        var font = Content.Load<SpriteFont>("font");
        Globals.Init(content: Content, spriteBatch: new SpriteBatch(GraphicsDevice), font);
        
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _log.LogInformation("Loading textures");
        _gm.AddTexture(Textures.Player);
        _gm.AddTexture(Textures.TestTile);
        _gm.AddTexture(Textures.TestBars);
        _gm.AddTexture(Textures.Collider);
        _gm.AddTexture(Textures.OpaqueBg);
        _gm.AddTexture(Textures.TransparentBg);
        
        _gui.InitBgTextures();
        _gm.TempInitPlayerWorld();
    }

    protected override void Update(GameTime gameTime)
    {
        var now = gameTime.TotalGameTime.TotalMilliseconds;
        _gui.Update();

        if (_lastUpTime == 0 || now - _lastUpTime >= UpdateTimeDelta)
        {
            _gm.FixedUpdate(gameTime);
            _lastUpTime = now;
        }
        
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        _gm.Draw();
        base.Draw(gameTime);
    }
}