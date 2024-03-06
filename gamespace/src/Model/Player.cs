using System;
using gamespace.Managers;
using Microsoft.Xna.Framework;

namespace gamespace.Model;
/// <summary>
/// A class to generate and store our player.
/// </summary>
public class Player : Character
{
    /// <summary>
    /// A constant to provide the size of our players usable inventory.
    /// </summary>
    private const int InventorySize = 5;
    
    /// <summary>
    /// A constant to provide the size of our players key item inventory.
    /// </summary>
    private const int KeyInventorySize = 4;

    /// <summary>
    /// A string to store the name of our player.
    /// </summary>
    private string _name;
    
    /// <summary>
    /// An array to store our players usable items.
    /// </summary>
    private Item[] _inventory;
    
    /// <summary>
    /// An array to store our players key items.
    /// </summary>
    private Item[] _keyItemInventory;
    
    /// <summary>
    /// A public property to access our players usable inventory.
    /// </summary>
    public Item[] Inventory => _inventory;
    
    /// <summary>
    /// A public property to access our players key inventory.
    /// </summary>
    public Item[] KeyItemInventory => _keyItemInventory;

    /// <summary>
    /// A constructor to build our player.
    /// </summary>
    /// <param name="name"> The name our player should have. </param>
    /// <param name="world"> The world our player should be put into. </param>
    public Player(string name, World world)
        : base(Vector2.Zero, 1, 1, 100, 100, 10, world)
    {
        _name = name;
        _inventory = new Item[InventorySize];
        _keyItemInventory = new Item[KeyInventorySize];
    }

    public bool InventoryUse(int inventorySlot)
    {
        if (inventorySlot is < 0 or >= InventorySize)
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
    
    /// <summary>
    /// Adds an item to the first open slot of Key inventory if it is a key item, or to the first slot of usable inventory.
    /// </summary>
    /// <param name="newItem"> The item to place in our players inventory. </param>
    /// <returns> Returns false if the players inventory is full, true if there is an open slot. </returns>
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
    
    /// <summary>
    /// Provides a string representation of our player.
    /// </summary>
    /// <returns> A string representation of player. </returns>
    public override string ToString()
    {
        var result = base.ToString() + " Name: " + _name;
        return result;
    }
    // === EVENT HANDLING ===-------------------------------------------------------------------------------------------

    public void HandleMoveEvent(in Vector2 moveVec)
    {
        MoveSpeed = new Vector2(BaseMoveSpeed * moveVec.X, BaseMoveSpeed * moveVec.Y);
    }
}