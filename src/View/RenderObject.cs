using System;
using System.Collections.Generic;
using gamespace.Managers;
using gamespace.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace gamespace.View;

public class RenderObject
{
    private readonly Texture2D _texture;

    private readonly Guid? _entityId;

    private Vector2 _position;

    private float _layerDepth;

    private readonly Dictionary<AnimationAction, Animation> _animations = new();

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
        _log = Globals.LogFactory.CreateLogger<RenderObject>();
        _texture = texture;
        _position = new Vector2(worldPosition.X * 16, worldPosition.Y * 16);
        _layerDepth = layerDepth;
        _entityId = entityId;

        if (texture.Name != Textures.Player) return;
        // TODO: fix RenderObject!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        // Placeholder that will be removed once implementation is complete.
        _animations[GetAnimationAction(worldPosition)] =
            new Animation(texture, 16, 16, 4, 0.1f, GetAnimationAction(worldPosition));
    }

    private static AnimationAction GetAnimationAction(Vector2 direction)
    {
        var directions = new Dictionary<Vector2, AnimationAction>
        {
            { new Vector2(1, 0), AnimationAction.E },
            { new Vector2(-1, 0), AnimationAction.W },
            { new Vector2(0, -1), AnimationAction.N },
            { new Vector2(0, 1), AnimationAction.S }
        };

        return directions.GetValueOrDefault(direction, AnimationAction.S);
    }

    // TODO: Make appropriate changes to update animations whenever there is an input.
    public void Update(GameTime gameTime)
    {
        var direction = InputManager.Direction;

        var action = GetAnimationAction(direction);

        foreach (var animation in _animations.Values)
        {
            animation.Update(gameTime, action);
        }
    }

    public void Draw()
    {
        if (_texture.Name == Textures.Player)
        {
            foreach (var animation in _animations.Values)
            {
                Globals.SpriteBatch.Draw(_texture, _position, animation.SourceRectangle, Color.White);
            }
        }
        else
        {
            Globals.SpriteBatch.Draw(_texture, _position, Color.White);
        }
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