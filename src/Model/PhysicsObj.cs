using Microsoft.Xna.Framework;

namespace gamespace.Model;

public abstract class PhysicsObj
{
    private int _x;
    private int _y;
    private int _width;
    private int _height;
    private bool _canCollide;
    private bool _canMove;

    protected PhysicsObj(int x, int y, int width, int height, bool canCollide, bool canMove)
    {
        _x = x;
        _y = y;
        _width = width;
        this._height = height;
        this._canCollide = canCollide;
        this._canMove = canMove;
    }

    public abstract void Update(GameTime gameTime);
    
    //USING THIS TEMP FOR TESTING, can happily change this once I get a chance to discuss how to approach this problem in full
    // Could also get rid of setX and setY by implementing move() in here
    public void SetX(int newX)
    
    {
        _x = newX;
    }
    public void SetY(int newY)
    {
        _y = newY;
    }

    public int GetX()
    {
        return _x;
    }

    public int GetY()
    {
        return _y;
    }

    public int GetWidth()
    {
        return _width;
    }

    public int GetHeight()
    {
        return _height;
    }
}