using System;
using gamespace.Util;
using Microsoft.Xna.Framework;

namespace gamespace.Model.Entities;

/// <summary>
/// A class to generate and store our player.
/// </summary>
public class Player : Character
{
    /// <summary>
    /// Counter for mobs killed.
    /// </summary>
    public static int MobsKilledCounter;

    /// <summary>
    /// Counter for items picked up.
    /// </summary>
    public static int ItemsPickedUpCounter;

    /// <summary>
    /// A constant to provide the size of our players usable inventory.
    /// </summary>
    private const int InventorySize = 5;

    /// <summary>
    /// A constant to provide the size of our players key item inventory.
    /// </summary>
    private const int KeyInventorySize = 4;

    /// <summary>
    /// Shows how many key items the player holds.
    /// </summary>
    public int KeyItemsHeld { get; private set; }

    /// <summary>
    /// A string to store the name of our player.
    /// </summary>
    private readonly string _name;

    /// <summary>
    /// A public property to access our players usable inventory.
    /// </summary>
    public Item[] Inventory { get; } = new Item[InventorySize];

    /// <summary>
    /// A public property to access our players key inventory.
    /// </summary>
    public Item[] KeyItemInventory { get; } = new Item[KeyInventorySize];

    /// <summary>
    /// A constructor to build our player.
    /// </summary>
    /// <param name="name"> The name our player should have. </param>
    /// <param name="world"> The world our player should be put into. </param>
    public Player(string name, World world)
        : base(Vector2.Zero, 0.9f, 0.9f, 100, 100, 10, world)
    {
        _name = name;
        KeyItemsHeld = 0;
        OnPlayerStateEvent();
    }

    /// <summary>
    /// Uses the correct item in the inventory slot specified.
    /// </summary>
    /// <param name="inventorySlot">The inventory slot to use.</param>
    public bool InventoryUse(int inventorySlot)
    {
        if (inventorySlot is < 0 or >= InventorySize)
        {
            return false;
        }

        var wantedItem = Inventory[inventorySlot];
        if (wantedItem == null)
        {
            return false;
        }

        var wantedItemUse = wantedItem.ItemUse;
        Inventory[inventorySlot] = null;
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
        ItemsPickedUpCounter++;
        if (newItem.IsKeyItem)
        {
            var firstEmptyIndex = Array.IndexOf(KeyItemInventory, null);
            if (firstEmptyIndex < 0)
            {
                return false;
            }

            newItem.User = this;
            KeyItemInventory[firstEmptyIndex] = newItem;
            KeyItemsHeld += 1;
            return true;
        }
        else
        {
            var firstEmptyIndex = Array.IndexOf(Inventory, null);
            if (firstEmptyIndex < 0)
            {
                return false;
            }

            newItem.User = this;
            Inventory[firstEmptyIndex] = newItem;
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

    /// <summary>
    /// Handler for player move event.
    /// </summary>
    /// <param name="moveVec">The move vector.</param>
    public void HandleMoveEvent(in Vector2 moveVec)
    {
        MoveSpeed = new Vector2(BaseMoveSpeed * moveVec.X, BaseMoveSpeed * moveVec.Y);
        if (MoveSpeed != Vector2.Zero)
        {
            LastMovingDirection = MoveSpeed;
        }
    }

    /// <summary>
    /// Handler for using an item.
    /// </summary>
    /// <param name="index">Index of the item being used.</param>
    public void HandleItemUseEvent(in int index)
    {
        Console.Out.WriteLine(index);
        InventoryUse(index);
    }

    /// <summary>
    /// Event for the player state.
    /// </summary>
    public event EventHelper.PlayerStateEventHandler PlayerStateEvent;

    /// <summary>
    /// Invokes event for the player state.
    /// </summary>
    private void OnPlayerStateEvent()
    {
        var args = new EventHelper.PlayerState(Health, Energy, Inventory, KeyItemsHeld);
        PlayerStateEvent?.Invoke(args);
        if (Health <= 0)
        {
            Environment.Exit(0);
        }
    }

    /// <summary>
    /// Event for when the player dies.
    /// </summary>
    public event EventHelper.LoseGameHandler PlayerDeadHandler;

    /// <summary>
    /// Handler for the event when a player dies.
    /// </summary>
    protected override void OnDeath()
    {
        base.OnDeath();
        PlayerDeadHandler?.Invoke();
    }

    /// <summary>
    /// Event for when a shot collides with player.
    /// </summary>
    public event EventHelper.PlayerShootBullets ShotReceived;

    /// <summary>
    /// Invokes event when a shot is received.
    /// </summary>
    public void OnPlayerShootSender()
    {
        ShotReceived?.Invoke();
    }

    /// <summary>
    /// Fixed update for the player.
    /// </summary>
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        OnPlayerStateEvent();
    }
}