
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
        Matrix baseTransform = Matrix.CreateTranslation(-target.X - (target.Width / 2),
            -target.Y - (target.Height / 2), 0);
        Matrix adjustScreenOffSet = Matrix.CreateTranslation(Game1.ScreenWidth / 2, Game1.ScreenHeight / 2, 0);
        Transform = baseTransform * adjustScreenOffSet;
    }
}