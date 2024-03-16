using gamespace.Model.Entities;
using gamespace.Util;
using Microsoft.Xna.Framework;

namespace gamespace.Model.Props;

public class Altar : InteractableProp
{
    /// <summary>
    /// Creates an alter object.
    /// </summary>
    /// <param name="worldCoordinate">Coordinate of the alter.</param>
    /// <param name="width">Width of the alter.</param>
    /// <param name="height">Height of the alter.</param>
    /// <param name="hasCollision">If the alter has collision or not, by default it should not.</param>
    public Altar(Vector2 worldCoordinate, float width, float height, bool hasCollision) : base(worldCoordinate, width,
        height, hasCollision)
    {
    }

    /// <summary>
    /// Event for winning the game.
    /// </summary>
    public event EventHelper.WinGameHandler Win;

    /// <summary>
    /// Invokes an event when the player interacts with the alter.
    /// </summary>
    /// <param name="player">The player interacting with the alter.</param>
    public override void InteractWithPlayer(Player player)
    {
        if (player.KeyItemsHeld >= 4)
        {
            Win?.Invoke();
        }
    }
}