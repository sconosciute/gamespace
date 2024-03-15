using Microsoft.Xna.Framework;
using System;
using gamespace.Util;

namespace gamespace.Model;

//End the game tile
public class Altar : InteractableProp
{
    public Altar(Vector2 worldCoordinate, float width, float height, bool hasCollision) : base(worldCoordinate, width,
        height, hasCollision)
    {
    }

    public event EventHelper.WinGameHandler Win;

    public override void InteractWithPlayer(Player player)
    {
        if (player.KeyItemsHeld >= 4)
        {
            Win?.Invoke();
        }
    }
}