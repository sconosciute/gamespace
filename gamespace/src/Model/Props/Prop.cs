using Microsoft.Xna.Framework;

namespace gamespace.Model;

public class Prop : PhysicsObj
{
    public Prop(Vector2 worldCoordinate, float width, float height, bool hasCollision) 
        : base(worldCoordinate, width, height, false, false, hasCollision)
    {
    }

    public override void FixedUpdate()
    {
        //TODO: Does Prop ever need to actually update?
    }

    public override string ToString()
    {
        var result = "World Coordinate: " + WorldCoordinate + " Width: " + Width + " Height: " + Height +
                     " Has movement: False" +
                     " Has friction: False" + " Has collision: " + CanCollide;
        return result;
    }
}