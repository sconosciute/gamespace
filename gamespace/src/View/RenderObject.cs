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

    private Vector2 _oldPosition;

    private readonly float _layerDepth;

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
        _animations[GetAnimationAction(worldPosition)] =
            new Animation(texture, 16, 16, 4, 100.0f, GetAnimationAction(worldPosition));
    }

    private static AnimationAction GetAnimationAction(Vector2 direction)
    {
        var angle = Math.Atan2(direction.Y, direction.X) * (180 / Math.PI);

        if (angle < 0)
        {
            angle += 360;
        }

        return angle switch
        {
            >= 22.5 and < 67.5 => AnimationAction.Se,
            >= 67.5 and < 112.5 => AnimationAction.S,
            >= 112.5 and < 157.5 => AnimationAction.Sw,
            >= 157.5 and < 202.5 => AnimationAction.W,
            >= 202.5 and < 247.5 => AnimationAction.Nw,
            >= 247.5 and < 292.5 => AnimationAction.N,
            >= 292.5 and < 337.5 => AnimationAction.Ne,
            _ => AnimationAction.E
        };
    }

    public void Update(GameTime gameTime)
    {
        var direction = InputManager.Direction;

        var action = GetAnimationAction(direction);

        if (_position == _oldPosition) return;
        _oldPosition = _position;
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
                Globals.SpriteBatch.Draw(_texture, _position, animation.SourceRectangle, Color.White, 0f, 
                    Vector2.Zero, 1f, SpriteEffects.None, _layerDepth);
            }
        }
        else
        {
            // TODO: Fix problem where using LayerDepth causes issues for other textures other than player.
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