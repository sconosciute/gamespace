using gamespace.View;

namespace gamespace.Model;

public abstract class Entity : PhysicsObj
{
    private RenderObject _sprite;
    private int _moveSpeed;

    protected Entity(int moveSpeed, RenderObject sprite, int x, int y, int width, int height, bool canCollide,
        bool canMove) : base(x, y, width, height,canCollide,  canMove)
    {
        _moveSpeed = moveSpeed;
        _sprite = sprite;
    }

    public abstract void Move(int x, int y);
    
    //No idea why this property does not work with just get; will look into this
    public int MoveSpeed
    {
        get => _moveSpeed;
    }
}