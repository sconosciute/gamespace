using gamespace.Model.Entities;
using Microsoft.Xna.Framework;

namespace gamespace.Model.Props;

public class Chest : InteractableProp
{
    /// <summary>
    /// The item that the chest holds.
    /// </summary>
    private readonly Item _itemHeld;

    /// <summary>
    /// Creates a chest object to place in the world.
    /// </summary>
    /// <param name="worldCoordinate">The coordinate of the chest.</param>
    /// <param name="width">The width of the chest.</param>
    /// <param name="height">The height of the chest.</param>
    /// <param name="hasCollision">Determines if the chest has collision, by default it does not.</param>
    /// <param name="itemHeld">The item that the chest will hold.</param>
    public Chest(Vector2 worldCoordinate, float width, float height, bool hasCollision, Item itemHeld) : base(
        worldCoordinate, width, height, hasCollision)
    {
        _itemHeld = itemHeld;
    }

    /// <summary>
    /// Adds an item to the player's inventory when they interact with the chest.
    /// </summary>
    /// <param name="player">The player interacting with the chest.</param>
    public override void InteractWithPlayer(Player player)
    {
        player.AddToInventory(_itemHeld);
    }

    /// <summary>
    /// ToString representation of the coordinates and inventory of the chest.
    /// </summary>
    public override string ToString()
    {
        return WorldCoordinate + " Contains key Item: " + _itemHeld.IsKeyItem;
    }
}