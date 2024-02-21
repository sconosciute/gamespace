using System;
using System.Collections.Generic;
using gamespace.Model;
using gamespace.View;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace gamespace.Managers;

public class GameManager
{
    private const int WorldSize = 51;

    //16:9 Widescreen resolution suitable for 16px tile sizes.
    private const int VResWidth = 640;
    private const int VResHeight = 360;

    private readonly GraphicsDeviceManager _gfx;
    private readonly Camera _camera;
    private readonly Player _player;
    private readonly World _world;
    private readonly Dictionary<string, Texture2D> _textures = new();

    public GameManager(GraphicsDeviceManager graphics)
    {
        _gfx = graphics;
        TempSetRes();
        _world = new World(WorldSize, WorldSize);
        _player = new Player("dude", _world);
        _camera = new Camera(_player.EntityId, _gfx.GraphicsDevice, new Point(VResWidth, VResHeight));

        _player.EntityEvent += _camera.HandleEntityEvent;
    }

    private void TempSetRes()
    {
        _gfx.PreferredBackBufferWidth = 1920;
        _gfx.PreferredBackBufferHeight = 800;
        _gfx.ApplyChanges();
    }

    public void InitPlayerWorld()
    {
        Guid dummy = Guid.NewGuid();
        
        var robj = new RenderObject(texture: GetTexture(Textures.Player), worldPosition: _player.WorldCoordinate, layerDepth: LayerDepth.Foreground, entityId: _player.EntityId);
        _camera.RegisterRenderable(robj);
        _player.EntityEvent += robj.HandleEntityEvent;
        robj.Update(new GameTime());


        _world.TryPlaceTile(new Point(5, 5),BuildTile(new Vector2(5f, 5f), Build.Props.Wall));
        _world.TryPlaceTile(new Point(5, 6),BuildTile(new Vector2(5f, 6f), Build.Props.Wall));
        
        _world.TryPlaceTile(new Point(10, 5),BuildTile(new Vector2(10f, 5f), Build.Props.Wall));
        _world.TryPlaceTile(new Point(11, 5),BuildTile(new Vector2(11f, 5f), Build.Props.Wall));
        
        _world.TryPlaceTile(new Point(-5, 5),BuildTile(new Vector2(-5f, 5f), Build.Props.Wall));
        _world.TryPlaceTile(new Point(-5, 6),BuildTile(new Vector2(-5f, 6f), Build.Props.Wall));
        _world.TryPlaceTile(new Point(-4, 5),BuildTile(new Vector2(-4f, 5f), Build.Props.Wall));
        
        _world.TryPlaceTile(new Point(20, 5),BuildTile(new Vector2(20f, 5f), Build.Props.Wall));
        _world.TryPlaceTile(new Point(20, 6),BuildTile(new Vector2(20f, 6f), Build.Props.Wall));
        _world.TryPlaceTile(new Point(21, 5),BuildTile(new Vector2(21f, 5f), Build.Props.Wall));
    }

    /// <summary>
    /// Draws the state of the world and all entities to screen.
    /// </summary>
    public void Draw()
    {
        _camera.BeginFrame();
        _world.DebugDrawMap();
        _camera.DrawFrame();
    }

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

    public void FixedUpdate()
    {
        _player.FixedUpdate();
    }

    private Tile BuildTile(Vector2 worldPosition, PropBuilder buildCallback)
    {
        var prop = buildCallback.Invoke(this, worldPosition, out var renderable);
        _camera.RegisterRenderable(renderable);
        return new Tile(prop);
    }

    private delegate Prop PropBuilder(GameManager gm, Vector2 worldPosition, out RenderObject renderable);
    
}