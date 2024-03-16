using gamespace.Managers;
using gamespace.Util;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace gamespace;

public class Game1 : Game
{
    /// <summary>
    /// A frequency for a timer.
    /// </summary>
    private const int UpdateTimeDelta = (1000 / 30);
    
    /// <summary>
    /// The last up time of the update time.
    /// </summary>
    private double _lastUpTime;
    
    /// <summary>
    /// The main game manager.
    /// </summary>
    private readonly GameManager _gm;
    
    /// <summary>
    /// Debug logger.
    /// </summary>
    private readonly ILogger _log;
    
    /// <summary>
    /// The main manager for GUI elements.
    /// </summary>
    private readonly GuiManager _gui;
    
    /// <summary>
    /// Property for how much time spent in the dungeon.
    /// </summary>
    public static int SecondsInDungeon { get; private set; }

    /// <summary>
    /// Initializes the game manager and GUI.
    /// </summary>
    public Game1()
    {
        _log = Globals.LogFactory.CreateLogger<Game1>();

        SettingsManager.Init(_log);
        var graphics = SettingsManager.GenerateGraphics(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        _gm = new GameManager(graphics, this);
        _gui = _gm.InitGui();
    }

    /// <summary>
    /// Initializes the game/base.
    /// </summary>
    protected override void Initialize()
    {
        _log.LogInformation("Initializing Game");
        var font = Content.Load<SpriteFont>("font");
        Globals.Init(content: Content, spriteBatch: new SpriteBatch(GraphicsDevice), font);

        base.Initialize();
    }

    /// <summary>
    /// Loads all textures used in the game.
    /// </summary>
    protected override void LoadContent()
    {
        _log.LogInformation("Loading textures");
        _gm.AddTexture(Textures.Player);
        _gm.AddTexture(Textures.TestTile);
        _gm.AddTexture(Textures.TestBars);
        _gm.AddTexture(Textures.Collider);
        _gm.AddTexture(Textures.OpaqueBg);
        _gm.AddTexture(Textures.TransparentBg);
        _gm.AddTexture(Textures.RoomConnector);
        _gm.AddTexture(Textures.Chest);
        _gm.AddTexture(Textures.NormalChest);
        _gm.AddTexture(Textures.Cat);
        _gm.AddTexture(Textures.SquareWall);
        _gm.AddTexture(Textures.DiamondWall);
        _gm.AddTexture(Textures.Spike);
        _gm.AddTexture(Textures.DarkDiamondWall);
        _gm.AddTexture(Textures.DarkSquareWall);
        _gm.AddTexture(Textures.PotLarge);
        _gm.AddTexture(Textures.Bullet);

        _gui.InitBgTextures();
        _gm.TempInitPlayerWorld();
    }

    /// <summary>
    /// Fixed update check for the game.
    /// </summary>
    protected override void Update(GameTime gameTime)
    {
        _gui.Update();
        InputDriver.Update();
        SecondsInDungeon = gameTime.TotalGameTime.Seconds;
        var now = gameTime.TotalGameTime.TotalMilliseconds;
        if (_lastUpTime == 0 || now - _lastUpTime >= UpdateTimeDelta)
        {
            _gm.FixedUpdate();
            _lastUpTime = now;
        }

        _gm.AnimationUpdate(gameTime);
        base.Update(gameTime);
    }

    /// <summary>
    /// Draws game elements.
    /// </summary>
    protected override void Draw(GameTime gameTime)
    {
        _gm.Draw();
        base.Draw(gameTime);
    }
}