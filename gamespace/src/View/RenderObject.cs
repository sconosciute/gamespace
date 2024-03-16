using System;
using System.Collections.Generic;
using gamespace.Managers;
using gamespace.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace gamespace.View;

public class RenderObject
{
    /// <summary>
    /// The texture of the render object.
    /// </summary>
    private readonly Texture2D _texture;

    /// <summary>
    /// Property for getting the correct entity ID.
    /// </summary>
    public Guid? EntityId { get; init; }

    /// <summary>
    /// The object's current position.
    /// </summary>
    private Vector2 _position;

    /// <summary>
    /// The object's old position.
    /// </summary>
    private Vector2 _oldPosition;

    /// <summary>
    /// List of animation actions and the animation they show.
    /// </summary>
    private readonly Dictionary<AnimationAction, Animation> _animations = new();

    /// <summary>
    /// Property for the layer enums.
    /// </summary>
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

    /// <summary>
    /// Returns the correct action based on the move vector from the player.
    /// </summary>
    /// <param name="direction">The direction the player is moving.</param>
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

    /// <summary>
    /// Sends an update to the animation.
    /// </summary>
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

    /// <summary>
    /// Draws a texture.
    /// </summary>
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

    /// <summary>
    /// Entity event handler.
    /// </summary>
    /// <param name="sender">The entity ID.</param>
    /// <param name="args">The event.</param>
    public void HandleEntityEvent(in Guid sender, in EventHelper.EntityEventArgs args)
    {
        if (sender != EntityId) return;
        if (args.EventTopic != EventHelper.EntityEventType.Moved) return;
        _position = args.NewPosition;
        _position.X *= Globals.TileSize;
        _position.Y *= Globals.TileSize;
    }
    
    /// <summary>
    /// Entity unregister handler.
    /// </summary>
    public event EventHelper.EntityUnregisterHandler Handle;
    
    /// <summary>
    /// Invoke event to unrender an object.
    /// </summary>
    /// <param name="sender">The sender to unrender.</param>
    public void SendUnrender(in Guid sender)
    {
        Handle?.Invoke(this);
    }
}

/// <summary>
/// Enums for the layer an object should be drawn at.
/// </summary>
public enum Layer
{
    Foreground,
    Midground,
    Background
}