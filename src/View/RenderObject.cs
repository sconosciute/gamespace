using System;
using gamespace.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace gamespace.View;

public class RenderObject
{
    private readonly Texture2D _texture;

    private readonly Guid _entityId;

    private Vector2 _position;

    public RenderObject(Texture2D texture, Vector2 worldPosition, Guid entityId)
    {
        _texture = texture;
        _entityId = entityId;
        _position = new Vector2(worldPosition.X * 16, worldPosition.Y * 16);
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
            _position.X *= Globals.TileSize;
            _position.Y *= Globals.TileSize;
            Console.Out.WriteLine($"Updated player position to {args.NewPosition}");
        }
    }
}