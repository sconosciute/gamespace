using Microsoft.Xna.Framework;
using System;

namespace gamespace.Model;

//End the game tile
public class Alter : InteractableProp
{
    public Alter(Vector2 worldCoordinate, float width, float height, bool hasCollision) : base(worldCoordinate, width, height, hasCollision)
    {
    }

    public override void InteractWithPlayer(Player player)
    {
        if (player.KeyItemsHeld >= 4)
        {
            //End the game here
            Environment.Exit(0);
        }
    }
}