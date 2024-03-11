using System;
using System.Collections.Generic;
using gamespace.Model;
using gamespace.Util;
using gamespace.View;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace gamespace.Managers;

public class GameManager
{
    private const int WorldSize = 51;
    private readonly Game1 _game;
    private readonly GraphicsDeviceManager _gfxMan;
    private readonly Camera _camera;
    private GuiManager _gui;
    private readonly Player _player;
    private readonly World _world;
    private readonly Dictionary<string, Texture2D> _textures = new();
    private RenderObject _robj;
    private bool _playing;

    public bool GameIsPaused => !_playing;

    private WorldBuilder _worldBuilder;
    public GameManager(GraphicsDeviceManager graphics, Game1 game)
    {
        _game = game;
        _gfxMan = graphics;
        SetResolution(1920, 1080);
        _world = new World(WorldSize, WorldSize);
        _player = new Player("dude", _world);
        _camera = new Camera(_player.EntityId, _gfxMan.GraphicsDevice);
        _worldBuilder = new WorldBuilder(this, _camera, _world);
        _world.Entites.Add(_player);
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

    public void TempInitPlayerWorld()
    {
        _player.EntityEvent += _camera.HandleEntityEvent;
        _gui.RegisterControlledEntity(_player);
        _gui.OpenStatPanel();
        Guid dummy = Guid.NewGuid();
        _worldBuilder.BuildWorld();
        //_worldBuilder.BuildBasicWorld();
        _robj = new RenderObject(texture: GetTexture(Textures.Player), worldPosition: _player.WorldCoordinate,
            layerDepth: Layer.Foreground, entityId: _player.EntityId);
        _camera.RegisterRenderable(_robj);
        _player.EntityEvent += _robj.HandleEntityEvent;

        _worldBuilder.MobShooting += MobShootHandler;
        //_worldBuilder.MakeRoom();

    }

    public void RegisterPlayerListener(EventHelper.PlayerStateEventHandler handler)
    {
        _player.PlayerStateEvent += handler;
    }
    
    #endregion

    #region Game Loop

    /// <summary>
    /// Runs physics updates on all Entities.
    /// </summary>
    public void FixedUpdate()
    {
        if (_playing)
        {
            _player.FixedUpdate();
            _worldBuilder.UpdateWorld(_player);
        }
    }
    
    public void PlayerShootHandler()
    {
        var Bullet = Build.Projectiles.Bullet(this, _world, _player.WorldCoordinate, _player.MoveSpeed, _player.EntityId, out RenderObject robj); //_player.WorldCoordinate
        _camera.RegisterRenderable(robj);
        Bullet.EntityEvent += robj.HandleEntityEvent;
        
        Bullet.SendObjToRenderObj += robj.SendUnrender;
        robj.Handle += _camera.HandleUnrenderEvent;

        Bullet.SendObjToWorldBuilder += _worldBuilder.HandleBulletDeregister;
        _worldBuilder.livingBullets.Add(Bullet);
        _worldBuilder.NumberOfBulletsToIterate++;
        //Bullet.FixedUpdate();
    }

    private void MobShootHandler(in Mob newMob)
    {
        var dx = _player.WorldCoordinate.X * 0.1f;
        var dy = _player.WorldCoordinate.Y * 0.1f;
        Vector2 direction = new Vector2(dx, dy);
        var Bullet = Build.Projectiles.Bullet(this, _world, newMob.WorldCoordinate, direction, newMob.EntityId, out RenderObject robj); //_player.WorldCoordinate
        _camera.RegisterRenderable(robj);
        Bullet.EntityEvent += robj.HandleEntityEvent;
        
        Bullet.SendObjToRenderObj += robj.SendUnrender;
        robj.Handle += _camera.HandleUnrenderEvent;

        Bullet.SendObjToWorldBuilder += _worldBuilder.HandleBulletDeregister;
        _worldBuilder.livingBullets.Add(Bullet);
        _worldBuilder.NumberOfBulletsToIterate++;
        
    }
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
        
        _camera.RenderFrame();
        _gui.RenderGui();
    }
    
    #endregion

    #region Management

    public Texture2D GetTexture(string assetName)
    {
        if (_textures.TryGetValue(assetName, out var texture))
        {
            return texture;
        }

        throw new KeyNotFoundException($"Could not find {assetName} in textures");
    }

    public void AddTexture(string textureName)
    {
        var text = Globals.Content.Load<Texture2D>(textureName);
        _textures.Add(text.Name, text);
    }

    private void SetResolution(int width, int height)
    {
        _gfxMan.PreferredBackBufferWidth = width;
        _gfxMan.PreferredBackBufferHeight = height;
        _gfxMan.ApplyChanges();
        Globals.UpdateScale(_gfxMan.GraphicsDevice);
    }

    public void PauseGame()
    {
        _playing = false;
    }

    public void ResumeGame()
    {
        _playing = true;
    }

    public void ExitGame()
    {
        _game.Exit();
    }

    public void SaveGame()
    {
        //TODO: Save!
    }

    public void LoadGame()
    {
        //TODO: Load!
    }
    
    #endregion

    private Tile BuildTile(Vector2 worldPosition, PropBuilder buildCallback)
    {
        var prop = buildCallback.Invoke(this, worldPosition, out var renderable);
        _camera.RegisterRenderable(renderable);
        return new Tile(prop);
    }
    
    private delegate Prop PropBuilder(GameManager gm, Vector2 worldPosition, out RenderObject renderable);
}