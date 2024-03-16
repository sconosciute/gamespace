using Microsoft.Xna.Framework;

namespace gamespace.Model.Props;

public class Prop : PhysicsObj
{
    /// <summary>
    /// Creates a prop object.
    /// </summary>
    /// <param name="worldCoordinate">The coordinate of the prop.</param>
    /// <param name="width">Width of the prop.</param>
    /// <param name="height">Height of the prop.</param>
    /// <param name="hasCollision">Determines if the prop has collision.</param>
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