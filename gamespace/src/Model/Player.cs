using System;
using gamespace.Managers;
using Microsoft.Xna.Framework;

namespace gamespace.Model;

public class Player : Character
{
    private const int InventorySize = 5;
    private const int KeyInventorySize = 4;

    private string _name;
    private Item[] _inventory;
    private Item[] _keyItemInventory;

    public Item[] Inventory => _inventory;

    public Player(string name, World world)
        : base(Vector2.Zero, 1, 1, 100, 100, 10, world)
    {
        _name = name;
        _inventory = new Item[InventorySize];
        _keyItemInventory = new Item[KeyInventorySize];
    }

    public new void FixedUpdate()
    {
        var direction = InputManager.Direction;
        MoveSpeed = new Vector2(BaseMoveSpeed * direction.X, BaseMoveSpeed * direction.Y);

        base.FixedUpdate();
    }

    public bool InventoryUse(int inventorySlot)
    {
        if (inventorySlot is < 0 or > InventorySize)
        {
            return false;
        }

        var wantedItem = _inventory[inventorySlot];
        if (wantedItem == null)
        {
            return false;
        }

        var wantedItemUse = wantedItem.ItemUse;
        _inventory[inventorySlot] = null;
        wantedItemUse.Invoke();
        return true;
    }

    public bool AddToInventory(Item newItem)
    {
        if (newItem.IsKeyItem)
        {
            var firstEmptyIndex = Array.IndexOf(_keyItemInventory, null);
            if (firstEmptyIndex < 0)
            {
                return false; //inventory is full
            }

            newItem.User = this;
            _keyItemInventory[firstEmptyIndex] = newItem;
            return true;
        }
        else
        {
            var firstEmptyIndex = Array.IndexOf(_inventory, null);
            if (firstEmptyIndex < 0)
            {
                return false;
            }

            newItem.User = this;
            _inventory[firstEmptyIndex] = newItem;
            return true;
        }
    }

    public override string ToString()
    {
        var result = base.ToString() + " Name: " + _name;
        return result;
    }
    // === EVENT HANDLING ===-------------------------------------------------------------------------------------------

    public void HandleMoveEvent(Vector2 moveVec)
    {
        MoveSpeed = new Vector2(BaseMoveSpeed * moveVec.X, BaseMoveSpeed * moveVec.Y);
    }
}