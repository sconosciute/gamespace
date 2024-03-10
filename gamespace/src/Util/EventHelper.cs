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
        NavEsc
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
        public int Health { get; }
        public int Energy { get; }
        public Item[] Inventory { get; init; }

        public PlayerState(in int health, in int energy, in Item[] inventory)
        {
            Health = health;
            Energy = energy;
            Inventory = inventory;
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
    
    public delegate void PlayerStateEventHandler(in PlayerState state);
}