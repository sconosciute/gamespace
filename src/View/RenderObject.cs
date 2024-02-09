using System;
using gamespace.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace gamespace.View;

public class RenderObject
{
    private Texture2D _texture;

    private Guid _entityID;

    private Vector2 _position;

    public RenderObject(Texture2D texture, Vector2 position, Guid entityId)
    {
        _texture = texture;
        _entityID = entityId;
        _position = position;
    }
    
    public void Draw()
    {
        Globals.SpriteBatch.Draw(_texture, _position, Color.White);
    }
    
    public void HandleEntityEvent(Guid sender, EntityEventArgs args)
    {
        if (sender != _entityID) return;
        if (args.EventTopic.Equals(EntityEventType.Moved))
        {
            _position = args.NewPosition;
        }
    }
}