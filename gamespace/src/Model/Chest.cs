using Microsoft.Xna.Framework;

namespace gamespace.Model;

public class Chest : Prop
{
    private readonly Item _itemHeld;
    public Chest(Vector2 worldCoordinate, float width, float height, bool hasCollision, Item itemHeld) : base(worldCoordinate, width, height, hasCollision)
    {
        _itemHeld = itemHeld;
    }

    public void PickUpItem(Player player)
    {
        if (player.Inventory.Length != Player.InventorySize || _itemHeld.IsKeyItem)
        {
            player.AddToInventory(_itemHeld);
        }
    }
}