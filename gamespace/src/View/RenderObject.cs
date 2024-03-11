using System;
using System.Collections.Generic;
using gamespace.Managers;
using gamespace.Util;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace gamespace.View;

public class RenderObject
{
    private readonly Texture2D _texture;

    public Guid? EntityId { get; init; }

    private Vector2 _position;

    private Vector2 _oldPosition;

    private readonly Dictionary<AnimationAction, Animation> _animations = new();

    public Layer Layer { get; }

    /// <summary>
    /// Generates a new RenderObject which will track an Entity for position updates.
    /// </summary>
    /// <param name="texture">The texture or complete Atlas to display.</param>
    /// <param name="worldPosition">The world coordinate position of this renderable</param>
    /// <param name="layerDepth">The depth at which this texture should be drawn in scene <see cref="LayerDepth"/></param>
    /// <param name="entityId">An entity ID to listen for EntityEvents from.</param>
    public RenderObject(Texture2D texture, Vector2 worldPosition, Layer layerDepth, Guid? entityId = null)
    {
        _texture = texture;
        _position = new Vector2(worldPosition.X * 16, worldPosition.Y * 16);
        Layer = layerDepth;
        EntityId = entityId;

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
                Globals.SpriteBatch.Draw(_texture, _position, animation.SourceRectangle, Color.White);
            }
        }
        else
        {
            Globals.SpriteBatch.Draw(_texture, _position, Color.White);
        }
    }

    public void HandleEntityEvent(in Guid sender, in EventHelper.EntityEventArgs args)
    {
        if (sender != EntityId) return;
        if (args.EventTopic == EventHelper.EntityEventType.Moved)
        {
            _position = args.NewPosition;
            _position.X *= Globals.TileSize;
            _position.Y *= Globals.TileSize;
        }
    }
    public event EventHelper.EntityUnregisterHandler Handle;
    public void SendUnrender(in Guid sender)
    {
        Handle?.Invoke(this);
    }
}

public enum Layer
{
    Foreground,
    Midground,
    Background
}