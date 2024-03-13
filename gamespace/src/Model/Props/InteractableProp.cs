using Microsoft.Xna.Framework;

namespace gamespace.Model;

 public abstract class InteractableProp : Prop
{
    protected InteractableProp(Vector2 worldCoordinate, float width, float height, bool hasCollision) : base(worldCoordinate, width, height, hasCollision)
    {
        
    }

    public abstract void InteractWithPlayer(Player player);
}