﻿using System.Collections.Generic;
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
    private readonly Game _game;
    private readonly Camera _camera;
    private readonly Player _player;
    private readonly World _world;
    private readonly Dictionary<string, Texture2D> _textures = new();

    public GameManager(Game game, GraphicsDeviceManager graphics)
    {
        _game = game;
        _gfx = graphics;
        _world = new World(WorldSize, WorldSize);
        _player = new Player("dude", _world);
        _camera = new Camera(_player.EntityId, _gfx.GraphicsDevice, new Point(VResWidth, VResHeight));
    }

    private void initBaseRender()
    {
        
    }
    
    public Texture2D GetTexture(string assetName)
    {
        if (_textures.TryGetValue(assetName, out var texture))
        {
            return texture;
        }
        throw new KeyNotFoundException($"Could not find {assetName} in textures");
    }

    public void AddTexture(Texture2D texture)
    {
        _textures.Add(texture.Name, texture);
    }

    public void FixedUpdate()
    {
        _player.FixedUpdate();
    }

}