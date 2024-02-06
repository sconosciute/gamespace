using gamespace.View;
using Microsoft.Xna.Framework;

namespace gamespace.Model;

public class Prop : PhysicsObj
{
    Prop(Vector2 worldCoordinate, int width, int height) : base(worldCoordinate, width, height, false, false)
    {
    }

    public override void FixedUpdate()
    {
        //TODO: Does Prop ever need to actually update?
    }
}