using System;
using gamespace.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace gamespace.Model;

public class Player : Character
{
    private const int InventorySize = 5;

    private string _name;
    private Item[] _inventory;

    public Player(string name, World world)
        : base(Vector2.Zero, 1, 1, 100, 100, 10, world)
    {
        _name = name;
        _inventory = new Item[InventorySize];
    }

    public new void FixedUpdate()
    {
        var direction = InputManager.Direction;
        MoveSpeed = new Vector2(BaseMoveSpeed * direction.X * 3, BaseMoveSpeed * direction.Y * 3); //added times 3 for faster testing

        base.FixedUpdate();
    }

    public bool InventoryUse(int inventorySlot, ItemUsedCallback usedCallback)
    {
        if (inventorySlot is < 0 or > InventorySize)
        {
            return false;
        }

        var wantedItem = _inventory[inventorySlot];
        var wantedItemUse = wantedItem.ItemUse;
        _inventory[inventorySlot] = null;
        wantedItemUse.Invoke();
        return true;
    }

    public bool AddToInventory(Item newItem)
    {
        var firstEmptyIndex = Array.IndexOf(_inventory, null);
        if (firstEmptyIndex < 0)
        {
            return false; //inventory is full
        }

        _inventory[firstEmptyIndex] = newItem;
        return true;
    }
    
    // === EVENT HANDLING ===-------------------------------------------------------------------------------------------

    public void HandleMoveEvent(Vector2 moveVec)
    {
        MoveSpeed = new Vector2(BaseMoveSpeed * moveVec.X, BaseMoveSpeed * moveVec.Y);
    }
}