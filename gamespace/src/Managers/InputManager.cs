using System;
using System.Collections.Generic;
using System.Text.Json;
using gamespace.Util;
using gamespace.View;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace gamespace.Managers;

public sealed class InputManager
{
    /// <summary>
    /// The direction associated with input.
    /// </summary>
    //TODO: Make this instanced and remove the public static API for movement, require event handling.
    private static Vector2 _direction;
    
    /// <summary>
    /// Public getter to retrieve the direction.
    /// </summary>
    public static Vector2 Direction => _direction;

    /// <summary>
    /// Shows if the player is moving North.
    /// </summary>
    private bool _pMoveNorth;
    
    /// <summary>
    /// Shows if the player is moving South.
    /// </summary>
    private bool _pMoveSouth;
    
    /// <summary>
    /// Shows if the player is moving East.
    /// </summary>
    private bool _pMoveEast;
    
    /// <summary>
    /// Shows if the player is moving West.
    /// </summary>
    private bool _pMoveWest;
    
    /// <summary>
    /// Dictionary containing the key pressed and the event linked to it.
    /// </summary>
    private readonly Dictionary<Keys, EventHelper.InputCallback> _keyMap;
    
    /// <summary>
    /// List of default key binds and their events.
    /// </summary>
    private readonly List<(Keys, EventHelper.InputCallback, EventHelper.InputCallbacks)> _defaultBinds = new();
    
    /// <summary>
    /// Dictionary that can trace back to the enum.
    /// </summary>
    private readonly Dictionary<EventHelper.InputCallback, EventHelper.InputCallbacks> _callbackToEnum = new();
    
    /// <summary>
    /// Dictionary that can trace back to the callback.
    /// </summary>
    private readonly Dictionary<EventHelper.InputCallbacks, EventHelper.InputCallback> _enumToCallback = new();

    /// <summary>
    /// The instance of input.
    /// </summary>
    private static InputManager _instance;
    
    /// <summary>
    /// Debug logger.
    /// </summary>
    private readonly ILogger _log;

    /// <summary>
    /// Initializes all key binds.
    /// </summary>
    private InputManager()
    {
        _log = Globals.LogFactory.CreateLogger<InputManager>();
        _instance = this;
        InitDefaultBindingsList();
        _keyMap = new Dictionary<Keys, EventHelper.InputCallback>();
        InitBindingsMaps();
        WriteKeyBindConf();

        if (!SettingsManager.TryReadConfig(ConfNames.KeyBinds, out var data)) return;

        _log.LogInformation("Found custom key binds, loading...");
        var keyMap = JsonSerializer.Deserialize<Dictionary<Keys, EventHelper.InputCallbacks>>(data);
        foreach (var bind in keyMap)
        {
            _keyMap.Remove(bind.Key);
            _keyMap.Add(bind.Key, _enumToCallback[bind.Value]);
        }

        _log.LogInformation("Keybinds successfully loaded as \n {binds}", _keyMap);
    }

    #region Default Binding Initialization

    /// <summary>
    /// Binds certain keys to their callbacks.
    /// </summary>
    private void InitBindingsMaps()
    {
        foreach (var bind in _defaultBinds)
        {
            _keyMap.Add(bind.Item1, bind.Item2);
        }

        foreach (var call in _defaultBinds)
        {
            _callbackToEnum.Add(call.Item2, call.Item3);
        }

        foreach (var call in _defaultBinds)
        {
            _enumToCallback.Add(call.Item3, call.Item2);
        }
    }

    /// <summary>
    /// Creates default key binds.
    /// </summary>
    private void InitDefaultBindingsList()
    {
        _defaultBinds.Add((Keys.W, MoveNorth, EventHelper.InputCallbacks.MoveUp));
        _defaultBinds.Add((Keys.S, MoveSouth, EventHelper.InputCallbacks.MoveDown));
        _defaultBinds.Add((Keys.A, MoveWest, EventHelper.InputCallbacks.MoveLeft));
        _defaultBinds.Add((Keys.D, MoveEast, EventHelper.InputCallbacks.MoveRight));
        _defaultBinds.Add((Keys.Add, ZoomIn, EventHelper.InputCallbacks.ZoomIn));
        _defaultBinds.Add((Keys.Subtract, ZoomOut, EventHelper.InputCallbacks.ZoomOut));
        _defaultBinds.Add((Keys.OemPlus, ZoomRst, EventHelper.InputCallbacks.ZoomRst));
        _defaultBinds.Add((Keys.Up, NavUp, EventHelper.InputCallbacks.NavUp));
        _defaultBinds.Add((Keys.Down, NavDown, EventHelper.InputCallbacks.NavDown));
        _defaultBinds.Add((Keys.Enter, NavSelect, EventHelper.InputCallbacks.NavSelect));
        _defaultBinds.Add((Keys.Escape, NavEsc, EventHelper.InputCallbacks.NavEsc));
        _defaultBinds.Add((Keys.OemTilde, NavTilde, EventHelper.InputCallbacks.NavTilde ));
        
        _defaultBinds.Add((Keys.D1, UseSlotItem1, EventHelper.InputCallbacks.InventorySlot1));
        _defaultBinds.Add((Keys.D2, UseSlotItem2, EventHelper.InputCallbacks.InventorySlot2));
        _defaultBinds.Add((Keys.D3, UseSlotItem3, EventHelper.InputCallbacks.InventorySlot3));
        _defaultBinds.Add((Keys.D4, UseSlotItem4, EventHelper.InputCallbacks.InventorySlot4));
        _defaultBinds.Add((Keys.D5, UseSlotItem5, EventHelper.InputCallbacks.InventorySlot5));
        
        _defaultBinds.Add((Keys.Space, ShootBullet, EventHelper.InputCallbacks.ShootBullet));
    }

    #endregion

    /// <summary>
    /// Sets the key bind.
    /// </summary>
    /// <param name="key">The key that needs to be pressed for the action.</param>
    /// <param name="callback">The event callback.</param>
    public void SetKeyBind(in Keys key, in EventHelper.InputCallbacks callback)
    {
        var func = _enumToCallback[callback];
        _keyMap.Remove(key);
        _keyMap.Add(key, func);
        WriteKeyBindConf();
    }

    /// <summary>
    /// Writes the key bind configuration file.
    /// </summary>
    private void WriteKeyBindConf()
    {
        var bindings = new Dictionary<Keys, EventHelper.InputCallbacks>();
        foreach (var bind in _keyMap)
        {
            var name = _callbackToEnum[bind.Value];
            bindings.Add(bind.Key, name);
        }

        var json = JsonSerializer.Serialize(bindings);
        SettingsManager.TryWriteConfig(ConfNames.KeyBinds, json);
    }

    /// <summary>
    /// Gets the input manager.
    /// </summary>
    public static InputManager GetInputManager()
    {
        return _instance ??= new InputManager();
    }

    /// <summary>
    /// Updates direction based on input.
    /// </summary>
    public void Update()
    {
        var oldDir = _direction;
        _direction = Vector2.Zero;
        

        if (_pMoveNorth) _direction.Y--;
        if (_pMoveSouth) _direction.Y++;
        if (_pMoveWest) _direction.X--;
        if (_pMoveEast) _direction.X++;

        if (_direction == oldDir) return;
        if (_direction != Vector2.Zero)
        {
            _direction.Normalize();
        }

        OnMoveEvent(_direction);
    }

    /// <summary>
    /// Handles keyboard events.
    /// </summary>
    public void HandleKeyboardEvent(in InputDriver.KeyEvent args)
    {
        if (_keyMap.TryGetValue(args.Key, out var callback))
        {
            callback?.Invoke(args.Action);
        }
    }

    #region Event Dispatch

    /// <summary>
    /// Player move event dispatch.
    /// </summary>
    public event EventHelper.MoveInputHandler MoveEvent;

    /// <summary>
    /// Invoke event based on the move vector.
    /// </summary>
    private void OnMoveEvent(in Vector2 moveVec)
    {
        MoveEvent?.Invoke(moveVec);
    }

    /// <summary>
    /// Zoom event dispatch.
    /// </summary>
    public event EventHelper.ZoomEventHandler ZoomEvent;

    /// <summary>
    /// Invoke event to zoom.
    /// </summary>
    private void OnZoomEvent(ZoomEventType zm)
    {
        ZoomEvent?.Invoke(zm);
    }

    /// <summary>
    /// Input event dispatch.
    /// </summary>
    public event EventHelper.InputEventHandler InputEvent;

    /// <summary>
    /// Invoke event based on the navigation given.
    /// </summary>
    private void OnNavEvent(in EventHelper.NavigationEvents nav)
    {
        InputEvent?.Invoke(nav);
    }

    /// <summary>
    /// Use item dispatch.
    /// </summary>
    public event EventHelper.PlayerUseItemEventHandler ItemUseEvent;

    /// <summary>
    /// Invoke using an item based on the key pressed.
    /// </summary>
    private void OnItemEvent(in int index)
    {
        ItemUseEvent?.Invoke(index);
    }

    /// <summary>
    /// Player shoot dispatch.
    /// </summary>
    public event EventHelper.PlayerShootBullets PlayerShootEvent;

    /// <summary>
    /// Invoke event if the player shoots.
    /// </summary>
    private void OnPlayerShootEvent()
    {
        PlayerShootEvent?.Invoke();
    }

    #endregion

    #region Input Callbacks

    /// <summary>
    /// Event for moving West.
    /// </summary>
    /// <param name="action">The key that invokes the action.</param>
    private void MoveWest(in InputDriver.KeyAction action) => _pMoveWest = action == InputDriver.KeyAction.Pressed;
    
    /// <summary>
    /// Event for moving East.
    /// </summary>
    /// <param name="action">The key that invokes the action.</param>
    private void MoveEast(in InputDriver.KeyAction action) => _pMoveEast = action == InputDriver.KeyAction.Pressed;
    
    /// <summary>
    /// Event for moving North.
    /// </summary>
    /// <param name="action">The key that invokes the action.</param>
    private void MoveNorth(in InputDriver.KeyAction action) => _pMoveNorth = action == InputDriver.KeyAction.Pressed;
    
    /// <summary>
    /// Event for moving South.
    /// </summary>
    /// <param name="action">The key that invokes the action.</param>
    private void MoveSouth(in InputDriver.KeyAction action) => _pMoveSouth = action == InputDriver.KeyAction.Pressed;
    
    /// <summary>
    /// Event for zooming in.
    /// </summary>
    /// <param name="action">The key that invokes the action.</param>
    private void ZoomIn(in InputDriver.KeyAction action) => 
        PressFilter(action, () => OnZoomEvent(ZoomEventType.Up));
    
    /// <summary>
    /// Event for zooming out.
    /// </summary>
    /// <param name="action">The key that invokes the action.</param>
    private void ZoomOut(in InputDriver.KeyAction action) => 
        PressFilter(action, () => OnZoomEvent(ZoomEventType.Down));
    
    /// <summary>
    /// Event for resetting zoom.
    /// </summary>
    /// <param name="action">The key that invokes the action.</param>
    private void ZoomRst(in InputDriver.KeyAction action) => 
        PressFilter(action, () => OnZoomEvent(ZoomEventType.Reset));
    
    /// <summary>
    /// Event for navigating up in the menu.
    /// </summary>
    /// <param name="action">The key that invokes the action.</param>
    private void NavUp(in InputDriver.KeyAction action) => 
        PressFilter(action, () => OnNavEvent(EventHelper.NavigationEvents.Up));
    
    /// <summary>
    /// Event for navigating down in the menu.
    /// </summary>
    /// <param name="action">The key that invokes the action.</param>
    private void NavDown(in InputDriver.KeyAction action) => 
        PressFilter(action, () => OnNavEvent(EventHelper.NavigationEvents.Down));
    
    /// <summary>
    /// Event for selecting in the menu.
    /// </summary>
    /// <param name="action">The key that invokes the action.</param>
    private void NavSelect(in InputDriver.KeyAction action) =>
        PressFilter(action, () => OnNavEvent(EventHelper.NavigationEvents.Select));
    
    /// <summary>
    /// Event for exiting in the menu.
    /// </summary>
    /// <param name="action">The key that invokes the action.</param>
    private void NavEsc(in InputDriver.KeyAction action) => 
        PressFilter(action, () => OnNavEvent(EventHelper.NavigationEvents.Escape));
    
    /// <summary>
    /// Event for opening the debug menu.
    /// </summary>
    /// <param name="action">The key that invokes the action.</param>
    private void NavTilde(in InputDriver.KeyAction action) =>
        PressFilter(action, () => OnNavEvent(EventHelper.NavigationEvents.Debug));
    
    /// <summary>
    /// The press filter when a button is pressed.
    /// </summary>
    /// <param name="action">The key that invokes the action.</param>
    /// <param name="callback">The action to call</param>
    private static void PressFilter(InputDriver.KeyAction action, Action callback)
    {
        if (action == InputDriver.KeyAction.Pressed) callback.Invoke();
    }
    
    /// <summary>
    /// Uses item in slot 1.
    /// </summary>
    /// <param name="action">The key that invokes the action.</param>
    private void UseSlotItem1(in InputDriver.KeyAction action) => PressFilter(action, () => OnItemEvent(0));
    
    /// <summary>
    /// Uses item in slot 2.
    /// </summary>
    /// <param name="action">The key that invokes the action.</param>
    private void UseSlotItem2(in InputDriver.KeyAction action) => PressFilter(action, () => OnItemEvent(1));
    
    /// <summary>
    /// Uses item in slot 3.
    /// </summary>
    /// <param name="action">The key that invokes the action.</param>
    private void UseSlotItem3(in InputDriver.KeyAction action) => PressFilter(action, () => OnItemEvent(2));
    
    /// <summary>
    /// Uses item in slot 4.
    /// </summary>
    /// <param name="action">The key that invokes the action.</param>
    private void UseSlotItem4(in InputDriver.KeyAction action) => PressFilter(action, () => OnItemEvent(3));
    
    /// <summary>
    /// Uses item in slot 5.
    /// </summary>
    /// <param name="action">The key that invokes the action.</param>
    private void UseSlotItem5(in InputDriver.KeyAction action) => PressFilter(action, () => OnItemEvent(4));

    /// <summary>
    /// Shoots a bullet.
    /// </summary>
    /// <param name="action">The key that invokes the action.</param>
    private void ShootBullet(in InputDriver.KeyAction action) => PressFilter(action, OnPlayerShootEvent);

    #endregion
}