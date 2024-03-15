using gamespace.Model.Entities;
using Microsoft.Xna.Framework;

namespace gamespace.Model.Props;

public class Spikes : InteractableProp
{
    /// <summary>
    /// Creates a spike tile object.
    /// </summary>
    /// <param name="worldCoordinate">The world coordinate of the spike.</param>
    /// <param name="width">Width of the spikes.</param>
    /// <param name="height">Height of the spikes.</param>
    /// <param name="hasCollision">Determines if the spikes have collision.</param>
    public Spikes(Vector2 worldCoordinate, float width, float height, bool hasCollision) : base(worldCoordinate, width, height, hasCollision)
    {
        
    }

    /// <summary>
    /// Damages the player if they walk onto the spikes.
    /// </summary>
    /// <param name="player">The player that interacts with the spikes.</param>
    public override void InteractWithPlayer(Player player)
    {
        player.AddHealth(-1);
    }
}