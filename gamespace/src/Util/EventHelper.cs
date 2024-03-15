using System;
using gamespace.Model;
using gamespace.Model.Entities;
using gamespace.View;
using Microsoft.Xna.Framework;

namespace gamespace.Util;

public static class EventHelper
{
    #region enums

    public enum PlayerCommand
    {
        Use
    }

    public enum EntityEventType
    {
        Moved
    }

    public enum NavigationEvents
    {
        Up,
        Down,
        Select,
        Escape,
        Debug
    }

    public enum InputCallbacks
    {
        MoveLeft,
        MoveRight,
        MoveUp,
        MoveDown,
        ZoomIn,
        ZoomOut,
        ZoomRst,
        NavUp,
        NavDown,
        NavSelect,
        NavEsc,
        NavTilde,
        InventorySlot1,
        InventorySlot2,
        InventorySlot3,
        InventorySlot4,
        InventorySlot5,
        ShootBullet
    }

    #endregion

    #region Helper Classes

    public class EntityEventArgs
    {
        /// <summary>
        /// The type/topic of this event.
        /// </summary>
        public EntityEventType EventTopic { get; init; }

        /// <summary>
        /// The previous position of this Entity.
        /// </summary>
        public Vector2 OldPosition { get; init; }

        /// <summary>
        /// The new position of this Entity.
        /// </summary>
        public Vector2 NewPosition { get; init; }
    }

    public class PlayerState
    {
        /// <summary>
        /// Property for player health.
        /// </summary>
        public int Health { get; init; }
        
        /// <summary>
        /// Property for player energy.
        /// </summary>
        public int Energy { get; init; }
        
        /// <summary>
        /// Property for player inventory.
        /// </summary>
        public Item[] Inventory { get; init; }
        
        /// <summary>
        /// Property for how many key items the player has.
        /// </summary>
        public int KeyItems { get; init; }

        /// <summary>
        /// Constructor for the current player state.
        /// </summary>
        /// <param name="health">The player's health.</param>
        /// <param name="energy">The player's energy.</param>
        /// <param name="inventory">The player's inventory.</param>
        /// <param name="keyCount">The count of key items the player has.</param>
        public PlayerState(in int health, in int energy, in Item[] inventory, in int keyCount)
        {
            Health = health;
            Energy = energy;
            Inventory = inventory;
            KeyItems = keyCount;
        }
    }
    
    public class PlayerPayload
    {
        /// <summary>
        /// Property for the correct item index.
        /// </summary>
        public int ItemIndex { get; init; }
    }

    #endregion

    #region delegates

    /// <summary>
    /// Handler for player actions/commands.
    /// </summary>
    /// <param name="cmd">The command associated with the player action.</param>
    /// <param name="payload">The player payload.</param>
    public delegate void PlayerCommandHandler(in PlayerCommand cmd, in PlayerPayload payload);

    /// <summary>
    /// Player move event handler type.
    /// </summary>
    public delegate void MoveInputHandler(in Vector2 moveVec);

    /// <summary>
    /// Zoom event handler type.
    /// </summary>
    public delegate void ZoomEventHandler(in ZoomEventType zm);

    /// <summary>
    /// Input event handler type.
    /// </summary>
    public delegate void InputEventHandler(in NavigationEvents nav);

    /// <summary>
    /// Input callback handler type.
    /// </summary>
    public delegate void InputCallback(in InputDriver.KeyAction action);

    /// <summary>
    /// Entity event handler type.
    /// </summary>
    public delegate void EntityEventHandler(in Guid sender, in EntityEventArgs args);

    /// <summary>
    /// Camera event handler type.
    /// </summary>
    public delegate void CameraEventHandler(in Matrix scale);

    /// <summary>
    /// Player use item event handler type.
    /// </summary>
    public delegate void PlayerUseItemEventHandler(in int index);

    /// <summary>
    /// Player state event handler type.
    /// </summary>
    public delegate void PlayerStateEventHandler(in PlayerState state);

    /// <summary>
    /// Entity unregister handler type.
    /// </summary>
    public delegate void EntityUnregisterHandler(RenderObject robj);

    /// <summary>
    /// Unrender entity handler type.
    /// </summary>
    public delegate void SendEntityToUnrender(in Guid sender);

    /// <summary>
    /// Send mob to world builder handler type.
    /// </summary>
    public delegate void SendMobToWorldBuilder(in Mob mobState);

    /// <summary>
    /// Player shooting bullets handler type.
    /// </summary>
    public delegate void PlayerShootBullets();

    /// <summary>
    /// Win game handler type.
    /// </summary>
    public delegate void WinGameHandler();

    /// <summary>
    /// Lose game handler type.
    /// </summary>
    public delegate void LoseGameHandler();

    #endregion
}