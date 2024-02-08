
using System;
using gamespace.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace gamespace.View;

/**
 * A new class to handle the camera
 */

//USING THIS DOCUMENTATION FOR HOW VIEWPORTS WORK https://monogame.net/api/Microsoft.Xna.Framework.Graphics.Viewport/
//USED THIS FOR BASIS OF CAMERA https://stackoverflow.com/questions/60148136/camera-not-centre-on-player-sprite-monogame
public class Camera
{
    private readonly Viewport _viewport; 
    private Vector2 _position;
    private readonly Guid _playerId;
    
    public Matrix Translation { get; private set; }

    public Camera(Guid playerId)
    {
        _playerId = playerId;
        
    }
    
    public Matrix Transform { get; private set;  }
    
    public void centerOn(Player target)
    {
        var x = target.WorldCoordinate.X;
        var y = target.WorldCoordinate.Y;
        Matrix baseTransform = Matrix.CreateTranslation(-x - (target.Width / 2),
            -y - (target.Height / 2), 0);
        Matrix adjustScreenOffSet = Matrix.CreateTranslation(Game1.ScreenWidth / 2, Game1.ScreenHeight / 2, 0);
        Transform = baseTransform * adjustScreenOffSet;
    }

    public void HandleEntityEvent(Guid sender, EntityEventArgs args)
    {
        if (sender != _playerId) return;
        if (args.Type != EntityEventType.Moved) return;
        UpdateTranslationMatrix(args.NewPosition);
    }

    private void UpdateTranslationMatrix(Vector2 position)
    {
        var dx = (Globals.WindowSize.X / 2) + (position.X * Globals.TileSize);
        var dy = (Globals.WindowSize.Y / 2) + (position.Y * 16);
        // var dx = (Globals.WindowSize.X / 2) - _hero.Position.X;
        // dx = MathHelper.Clamp(dx, -_map.MapSize.X + Globals.WindowSize.X + (_map.TileSize.X / 2), _map.TileSize.X / 2);
        // var dy = (Globals.WindowSize.Y / 2) - _hero.Position.Y;
        // dy = MathHelper.Clamp(dy, -_map.MapSize.Y + Globals.WindowSize.Y + (_map.TileSize.Y / 2), _map.TileSize.Y / 2);
        // _translation = Matrix.CreateTranslation(dx, dy, 0f);

        Translation = Matrix.CreateTranslation(dx, dy, 0);
    }
    
}