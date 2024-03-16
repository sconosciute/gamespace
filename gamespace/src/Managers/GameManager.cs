using System.Collections.Generic;
using gamespace.Managers.BusinessPark;
using gamespace.Managers.Database;
using gamespace.Model;
using gamespace.Model.Entities;
using gamespace.Model.Props;
using gamespace.Util;
using gamespace.View;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace gamespace.Managers;

public class GameManager
{
    /// <summary>
    /// The world's width and height.
    /// </summary>
    private const int WorldSize = 51;
    
    /// <summary>
    /// Game object to use all functionalities of Monogame.
    /// </summary>
    private readonly Game1 _game;
    
    /// <summary>
    /// Graphics for the application window.
    /// </summary>
    private readonly GraphicsDeviceManager _gfxMan;
    
    /// <summary>
    /// Camera object for rendering all objects to be seen by the user.
    /// </summary>
    private readonly Camera _camera;
    
    /// <summary>
    /// GUI for menus.
    /// </summary>
    private GuiManager _gui;
    
    /// <summary>
    /// Player object for the camera to center on and can be interacted by the user.
    /// </summary>
    private readonly Player _player;
    
    /// <summary>
    /// The world for everything to be generated on.
    /// </summary>
    private readonly World _world;
    
    /// <summary>
    /// Dictionary containing all textures used for the game.
    /// </summary>
    private readonly Dictionary<string, Texture2D> _textures = new();
    
    /// <summary>
    /// Render object to draw anything that can be seen by the player.
    /// </summary>
    private RenderObject _robj;
    
    /// <summary>
    /// Boolean to see if the player is actively playing the game.
    /// </summary>
    private bool _playing;

    /// <summary>
    /// Boolean to see if the player is actively playing the game.
    /// </summary>
    public bool GameIsPaused => !_playing;

    /// <summary>
    /// World builder object for randomly generating the world.
    /// </summary>
    private readonly WorldBuilder _worldBuilder;
    
    /// <summary>
    /// The main menu that will contain save, load, close, and exit buttons.
    /// </summary>
    /// <param name="graphics">The current game's application window.</param>
    /// <param name="game">The current game.</param>
    public GameManager(GraphicsDeviceManager graphics, Game1 game)
    {
        _game = game;
        _gfxMan = graphics;
        SetResolution(1920, 1080);
        _world = new World(WorldSize, WorldSize);
        _player = new Player("dude", _world);
        _camera = new Camera(_player.EntityId, _gfxMan.GraphicsDevice);
        _worldBuilder = new WorldBuilder(this, _camera, _world);
        _world.Entities.Add(_player);
        _playing = true;
    }

    #region Init

    /// <summary>
    /// Initializes the GUI manager with relevant Camera information and returns that Manager.
    /// </summary>
    /// <returns>The GUI Manager related to this Game Manager.</returns>
    public GuiManager InitGui()
    {
        return _gui ??= new GuiManager(_gfxMan.GraphicsDevice, this, _camera);
    }

    /// <summary>
    /// Initializes the world and renders objects in.
    /// </summary>
    public void TempInitPlayerWorld()
    {
        DbHandler.InitDatabase();
        _player.EntityEvent += _camera.HandleEntityEvent;
        _gui.RegisterControlledEntity(_player);
        _gui.OpenPersistentElements();
        _worldBuilder.BuildWorld();
        _robj = new RenderObject(texture: GetTexture(Textures.Player), worldPosition: _player.WorldCoordinate,
            layerDepth: Layer.Foreground, entityId: _player.EntityId);
        _camera.RegisterRenderable(_robj);
        _player.EntityEvent += _robj.HandleEntityEvent;

        _worldBuilder.MobShooting += MobShootHandler;

    }

    /// <summary>
    /// Registers the listener to the player.
    /// </summary>
    /// <param name="handler">Handles the listener set to the player.</param>
    public void RegisterPlayerListener(EventHelper.PlayerStateEventHandler handler)
    {
        _player.PlayerStateEvent += handler;
        _player.PlayerDeadHandler += LoseGame;
    }
    
    #endregion

    #region Game Loop

    /// <summary>
    /// Runs physics updates on all Entities.
    /// </summary>
    public void FixedUpdate()
    {
        if (!_playing) return;
        _player.FixedUpdate();
        _worldBuilder.UpdateWorld(_player);
    }
    
    /// <summary>
    /// Registers and handles each bullet that the player fires.
    /// </summary>
    public void PlayerShootHandler()
    {
        var bullet = Build.Projectiles.Bullet(this, _world, _player.WorldCoordinate, _player.LastMovingDirection, _player.EntityId, out var robj);
        _camera.RegisterRenderable(robj);
        bullet.EntityEvent += robj.HandleEntityEvent;
        
        bullet.SendObjToRenderObj += robj.SendUnrender;
        robj.Handle += _camera.HandleUnrenderEvent;

        bullet.SendObjToWorldBuilder += _worldBuilder.HandleBulletDeregister;
        _worldBuilder.LivingBullets.Add(bullet);
        _worldBuilder.NumberOfBulletsToIterate++;
    }

    /// <summary>
    /// Registers and handles each bullet that the mob fires.
    /// </summary>
    private void MobShootHandler(in Mob newMob)
    {
        var dx = (_player.WorldCoordinate.X - newMob.WorldCoordinate.X) * 0.015f;
        var dy = (_player.WorldCoordinate.Y - newMob.WorldCoordinate.Y) * 0.015f;
        var direction = new Vector2(dx, dy);
        
        var bullet = Build.Projectiles.Bullet(this, _world, newMob.WorldCoordinate, direction, newMob.EntityId, out var robj);
        _camera.RegisterRenderable(robj);
        bullet.EntityEvent += robj.HandleEntityEvent;
        
        bullet.SendObjToRenderObj += robj.SendUnrender;
        robj.Handle += _camera.HandleUnrenderEvent;

        bullet.SendObjToWorldBuilder += _worldBuilder.HandleBulletDeregister;
        _worldBuilder.LivingBullets.Add(bullet);
        _worldBuilder.NumberOfBulletsToIterate++;
        
    }
    
    /// <summary>
    /// Updates animations using the game ticks.
    /// </summary>
    /// <param name="gameTime">The game time/ticks.</param>
    public void AnimationUpdate(GameTime gameTime)
    {
        _robj.Update(gameTime);
    }

    /// <summary>
    /// Draws the state of the world and all entities to screen.
    /// </summary>
    public void Draw()
    {
        _camera.BeginFrame();
        _camera.DrawFrame(RenderMode.Deferred);
        
        Camera.RenderFrame();
        _gui.RenderGui();
    }
    
    #endregion

    #region Management

    /// <summary>
    /// Retrieves the correct texture based on the string name you give.
    /// </summary>
    /// <param name="assetName">String name of the asset.</param>
    public Texture2D GetTexture(string assetName)
    {
        if (_textures.TryGetValue(assetName, out var texture))
        {
            return texture;
        }

        throw new KeyNotFoundException($"Could not find {assetName} in textures");
    }

    /// <summary>
    /// Adds textures to the texture class using the given string name that will be used to grab the texture from the content manager.
    /// </summary>
    /// <param name="textureName">The name you want to give the texture.</param>
    public void AddTexture(string textureName)
    {
        var text = Globals.Content.Load<Texture2D>(textureName);
        _textures.Add(text.Name, text);
    }

    /// <summary>
    /// Sets the correct resolution based on the height and width in pixels.
    /// </summary>
    /// <param name="width">The width of the application.</param>
    /// <param name="height">The height of the application.</param>
    private void SetResolution(int width, int height)
    {
        _gfxMan.PreferredBackBufferWidth = width;
        _gfxMan.PreferredBackBufferHeight = height;
        _gfxMan.ApplyChanges();
        Globals.UpdateScale(_gfxMan.GraphicsDevice);
    }

    /// <summary>
    /// Pauses the game.
    /// </summary>
    public void PauseGame()
    {
        _playing = false;
    }

    /// <summary>
    /// Resumes the game.
    /// </summary>
    public void ResumeGame()
    {
        _playing = true;
    }

    /// <summary>
    /// Exits the game.
    /// </summary>
    public void ExitGame()
    {
        _game.Exit();
    }

    /// <summary>
    /// Saves the game.
    /// </summary>
    public void SaveGame()
    {
        //TODO: Save!
    }

    /// <summary>
    /// Loads the game.
    /// </summary>
    public void LoadGame()
    {
        //TODO: Load!
    }
    
    /// <summary>
    /// Interacts with the database to add player stats once they win the game.
    /// </summary>
    public void WinGame()
    {
        //Save stats
        DbHandler.WriteStat(Statistic.Stats.ItemsFound, Player.ItemsPickedUpCounter);
        DbHandler.WriteStat(Statistic.Stats.MobKillCount, Player.MobsKilledCounter); 
        DbHandler.WriteStat(Statistic.Stats.TimeInDungeon, Game1.SecondsInDungeon); 
        DbHandler.WriteStat(Statistic.Stats.WinCount, 1); 
        ExitGame();
    }

    /// <summary>
    /// Interacts with the database to add player stats once they lose the game.
    /// </summary>
    private void LoseGame()
    {
        //Save stats
        DbHandler.WriteStat(Statistic.Stats.ItemsFound, Player.ItemsPickedUpCounter);
        DbHandler.WriteStat(Statistic.Stats.MobKillCount, Player.MobsKilledCounter); 
        DbHandler.WriteStat(Statistic.Stats.TimeInDungeon, Game1.SecondsInDungeon); 
        DbHandler.WriteStat(Statistic.Stats.DeathCount, 1); 
        ExitGame();
    }
    
    #endregion

    /// <summary>
    /// Builds a single tile.
    /// </summary>
    /// <param name="worldPosition">World position for the tile to be built on.</param>
    /// <param name="buildCallback">The build event.</param>
    private Tile BuildTile(Vector2 worldPosition, PropBuilder buildCallback)
    {
        var prop = buildCallback.Invoke(this, worldPosition, out var renderable);
        _camera.RegisterRenderable(renderable);
        return new Tile(prop);
    }
    
    /// <summary>
    /// Event to be sent to build props.
    /// </summary>
    /// <param name="gm">The current game to build it on.</param>
    /// <param name="worldPosition">The world position to build the prop.</param>
    /// <param name="renderable">The renderable prop is returned and drawn.</param>
    private delegate Prop PropBuilder(GameManager gm, Vector2 worldPosition, out RenderObject renderable);
}