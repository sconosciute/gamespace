using Microsoft.Xna.Framework;

namespace gamespace.Model;

public class Spikes : InteractableProp
{
    public Spikes(Vector2 worldCoordinate, float width, float height, bool hasCollision) : base(worldCoordinate, width, height, hasCollision)
    {
    }

    public override void InteractWithPlayer(Player player)
    {
        player.AddHealth(-5);
    }
}