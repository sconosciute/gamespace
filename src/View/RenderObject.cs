using System;
using gamespace.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace gamespace.View;

public class RenderObject
{
    private readonly Texture2D _texture;

    private readonly Guid _entityId;

    private Vector2 _position;

    public RenderObject(Texture2D texture, Vector2 position, Guid entityId)
    {
        _texture = texture;
        _entityId = entityId;
        _position = position;
    }
    
    public void Draw()
    {
        Globals.SpriteBatch.Draw(_texture, _position, Color.White);
    }
    
    public void HandleEntityEvent(Guid sender, EntityEventArgs args)
    {
        if (sender != _entityId) return;
        if (args.EventTopic.Equals(EntityEventType.Moved))
        {
            _position = args.NewPosition;
        }
    }
}