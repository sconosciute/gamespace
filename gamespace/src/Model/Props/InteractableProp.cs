using gamespace.Model.Entities;
using Microsoft.Xna.Framework;

namespace gamespace.Model.Props;

 public abstract class InteractableProp : Prop
{
    /// <summary>
    /// Creates an interactable prop object.
    /// </summary>
    /// <param name="worldCoordinate">Coordinate of the prop.</param>
    /// <param name="width">Width of the prop.</param>
    /// <param name="height">Height of the prop.</param>
    /// <param name="hasCollision">Determines if the prop has collision or not.</param>
    protected InteractableProp(Vector2 worldCoordinate, float width, float height, bool hasCollision) : base(worldCoordinate, width, height, hasCollision)
    {
        
    }

    /// <summary>
    /// Triggers when the prop is interacted by the player.
    /// </summary>
    /// <param name="player">The player that interacts with the prop.</param>
    public abstract void InteractWithPlayer(Player player);
}