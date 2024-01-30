using gamespace.View;

namespace gamespace.Model;

public class Prop : PhysicsObj
{
    private RenderObject _sprite;
    
    Prop(int moveSpeed, RenderObject sprite, int x, int y, int width, int height, bool canCollide,
        bool canMove) : base(x, y, width, height,canCollide,  canMove)
    {
        _sprite = sprite;
    }

    public override void Update()
    {
        throw new System.NotImplementedException();
    }
}