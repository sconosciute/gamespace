
using gamespace.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace gamespace.View;

/**
 * A new class to handle the camera
 */

//USING THIS DOCUMENTATION FOR HOW VIEWPORTS WORK https://monogame.net/api/Microsoft.Xna.Framework.Graphics.Viewport/
public class Camera
{
    private readonly Viewport _viewport; //Camera viewport
    private Vector2 _position;    //Camera Position Vector
    
    //May create a zoom, or transform?
    public Matrix Transform { get; private set;  }
    
    public void centerOn(Player target)
    {
        Matrix baseTransform = Matrix.CreateTranslation(-target.GetX() - (target.GetWidth() / 2),
            -target.GetY() - (target.GetHeight() / 2), 0);
        Matrix adjustScreenOffSet = Matrix.CreateTranslation(Game1.ScreenWidth / 2, Game1.ScreenHeight / 2, 0);
        Transform = baseTransform * adjustScreenOffSet;
    }
    /*public void SetPosition(int x, int y)
    {
        _position.X = x;
        _position.Y = y;
    }*/
}