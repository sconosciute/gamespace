﻿using System;
using gamespace.Managers;
using Microsoft.Xna.Framework;

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
        var wantedItemUse = wantedItem.GetItemUse(this);
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
}