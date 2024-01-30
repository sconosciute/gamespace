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

    public abstract void Update();
}