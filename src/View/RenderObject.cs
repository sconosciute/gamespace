using System;
using System.Collections.Generic;
using gamespace.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace gamespace.View;

public class RenderObject
{
    private readonly Texture2D _texture;

    private readonly Guid? _entityId;

    private Vector2 _position;

    private float _layerDepth;
    
    private readonly Dictionary<object, Animation> _animations = new();
    private object _lastKey;

    private ILogger _log;
        
    /// <summary>
    /// Generates a new RenderObject which will track an Entity for position updates.
    /// </summary>
    /// <param name="texture">The texture or complete Atlas to display.</param>
    /// <param name="worldPosition">The world coordinate position of this renderable</param>
    /// <param name="layerDepth">The depth at which this texture should be drawn in scene <see cref="LayerDepth"/></param>
    /// <param name="entityId">An entity ID to listen for EntityEvents from.</param>
    public RenderObject(Texture2D texture, Vector2 worldPosition, float layerDepth, Guid? entityId = null)
    {
        _log = Globals.LogFactory.CreateLogger <RenderObject> ();
        _texture = texture;
        _position = new Vector2(worldPosition.X * 16, worldPosition.Y * 16);
        _layerDepth = layerDepth;
        _entityId = entityId;
    }
    
    public void Draw()
    {
        Globals.SpriteBatch.Draw(_texture, _position, Color.White);
    }
    
    public void HandleEntityEvent(Guid sender, EntityEventArgs args)
    {
        if (sender != _entityId) return;
        if (args.EventTopic == EntityEventType.Moved)
        {
            _position = args.NewPosition;
            _position.X *= Globals.TileSize;
            _position.Y *= Globals.TileSize;
        }
    }

    public void AddAnimation(object key, Animation animation)
    {
        _animations.Add(key, animation);
        _lastKey ??= key;
    }

    public void Update(object key)
    {
        if (_animations.TryGetValue(key, out var value))
        {
            value.Start();
            _animations[key].Update();
            _lastKey = key;
        }
        else
        {
            _animations[_lastKey].Stop();
            _animations[_lastKey].Reset();
        }
    }

    public void Draw(Vector2 position)
    {
        _animations[_lastKey].Draw((position));
    }
}

/// <summary>
/// A helper to provide specific depths for SpriteBatch layers.
/// </summary>
public struct LayerDepth
{
    /// <summary>
    /// Objects in the foreground will be drawn over everything else. Place Entities here.
    /// </summary>
    public const float Foreground = 0f;
    
    /// <summary>
    /// Objects in the midground will be drawn between the other two layers. Props that render over the floor but under entities are best placed here.
    /// </summary>
    public const float Midground = 0.5f;
    
    /// <summary>
    /// Objects in the background will be drawn underneath all other objects.
    /// </summary>
    public const float Background = 1f;

}

public enum AnimationName
{
    N,
    Ne,
    E,
    Se,
    S,
    Sw,
    W,
    Nw,
    Use,
    Die
}