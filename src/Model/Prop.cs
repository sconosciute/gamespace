using gamespace.View;
using Microsoft.Xna.Framework;

namespace gamespace.Model;

public class Prop : PhysicsObj
{
    private RenderObject _sprite;
    
    Prop(int moveSpeed, RenderObject sprite, int x, int y, int width, int height, bool hasCollision,
        bool hasMovement) : base(x, y, width, height,hasCollision,  hasMovement)
    {
        _sprite = sprite;
    }

    public override void Update(GameTime gameTime)
    {
        throw new System.NotImplementedException();
    }
}