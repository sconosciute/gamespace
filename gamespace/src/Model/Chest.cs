using Microsoft.Xna.Framework;

namespace gamespace.Model;

public class Chest : InteractableProp
{
    private readonly Item _itemHeld;

    public Chest(Vector2 worldCoordinate, float width, float height, bool hasCollision, Item itemHeld) : base(
        worldCoordinate, width, height, hasCollision)
    {
        _itemHeld = itemHeld;
    }

    public override void InteractWithPlayer(Player player)
    {
        player.AddToInventory(_itemHeld);
    }

    public override string ToString()
    {
        return WorldCoordinate + " Contains key Item: " + _itemHeld.IsKeyItem;
    }
}