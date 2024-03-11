using System;
using gamespace.Managers;
using gamespace.Model;
using gamespace.View;
using Microsoft.Xna.Framework;

namespace gamespace.Util;

public static class EventHelper
{
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
        Escape
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
        InventorySlot1,
        InventorySlot2,
        InventorySlot3,
        InventorySlot4,
        InventorySlot5,
        ShootBullet
    }
    
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
        public int Health { get; init; }
        public int Energy { get; init; }
        public Item[] Inventory { get; init; }
        public int KeyItems { get; init; }

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
        public int ItemIndex { get; init; }
    }
    
    public delegate void PlayerCommandHandler(in PlayerCommand cmd, in PlayerPayload payload);

    /// <summary>
    /// Player move event handler type.
    /// </summary>
    public delegate void MoveInputHandler(in Vector2 moveVec);

    /// <summary>
    /// Zoom event handler type.
    /// </summary>
    public delegate void ZoomEventHandler(in ZoomEventType zm);

    public delegate void InputEventHandler(in NavigationEvents nav);

    public delegate void InputCallback(in InputDriver.KeyAction action);

    public delegate void EntityEventHandler(in Guid sender, in EntityEventArgs args);

    public delegate void CameraEventHandler(in Matrix scale);
    public delegate void PlayerUseItemEventHandler(in int index);
    public delegate void PlayerStateEventHandler(in PlayerState state);

    public delegate void EntityUnregisterHandler(RenderObject robj);

    public delegate void SendEntityToUnrender(in Guid sender);

    public delegate void PlayerShootBullets();
}