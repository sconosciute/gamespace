
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
    
    
    
    // var dx = (Globals.WindowSize.X / 2) - _hero.Position.X;
    // dx = MathHelper.Clamp(dx, -_map.MapSize.X + Globals.WindowSize.X + (_map.TileSize.X / 2), _map.TileSize.X / 2);
    // var dy = (Globals.WindowSize.Y / 2) - _hero.Position.Y;
    // dy = MathHelper.Clamp(dy, -_map.MapSize.Y + Globals.WindowSize.Y + (_map.TileSize.Y / 2), _map.TileSize.Y / 2);
    // _translation = Matrix.CreateTranslation(dx, dy, 0f);
}