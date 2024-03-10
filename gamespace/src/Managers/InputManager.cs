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
    //TODO: Make this instanced and remove the public static API for movement, require event handling.
    private static Vector2 _direction;
    public static Vector2 Direction => _direction;

    private bool _pMoveNorth;
    private bool _pMoveSouth;
    private bool _pMoveEast;
    private bool _pMoveWest;
    

    private readonly Dictionary<Keys, EventHelper.InputCallback> _keyMap;
    private readonly List<(Keys, EventHelper.InputCallback, EventHelper.InputCallbacks)> _defaultBinds = new();
    private readonly Dictionary<EventHelper.InputCallback, EventHelper.InputCallbacks> _callbackToEnum = new();
    private readonly Dictionary<EventHelper.InputCallbacks, EventHelper.InputCallback> _enumToCallback = new();

    private static InputManager _instance;
    private readonly ILogger _log;

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
        
        _defaultBinds.Add((Keys.D1, UseSlotItem1, EventHelper.InputCallbacks.InventorySlot1));
        _defaultBinds.Add((Keys.D2, UseSlotItem2, EventHelper.InputCallbacks.InventorySlot2));
        _defaultBinds.Add((Keys.D3, UseSlotItem3, EventHelper.InputCallbacks.InventorySlot3));
        _defaultBinds.Add((Keys.D4, UseSlotItem4, EventHelper.InputCallbacks.InventorySlot4));
        _defaultBinds.Add((Keys.D5, UseSlotItem5, EventHelper.InputCallbacks.InventorySlot5));
    }

    #endregion

    public void SetKeyBind(in Keys key, in EventHelper.InputCallbacks callback)
    {
        var func = _enumToCallback[callback];
        _keyMap.Remove(key);
        _keyMap.Add(key, func);
        WriteKeyBindConf();
    }

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

    public static InputManager GetInputManager()
    {
        return _instance ??= new InputManager();
    }

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

    private void OnMoveEvent(in Vector2 moveVec)
    {
        MoveEvent?.Invoke(moveVec);
    }

    /// <summary>
    /// Zoom event dispatch.
    /// </summary>
    public event EventHelper.ZoomEventHandler ZoomEvent;

    private void OnZoomEvent(ZoomEventType zm)
    {
        ZoomEvent?.Invoke(zm);
    }

    public event EventHelper.InputEventHandler InputEvent;

    private void OnNavEvent(in EventHelper.NavigationEvents nav)
    {
        InputEvent?.Invoke(nav);
    }

    public event EventHelper.PlayerUseItemEventHandler ItemUseEvent;

    private void OnItemEvent(in int index)
    {
        ItemUseEvent?.Invoke(index);
    }

    #endregion

    #region Input Callbacks

    private void MoveWest(in InputDriver.KeyAction action) => _pMoveWest = action == InputDriver.KeyAction.Pressed;
    private void MoveEast(in InputDriver.KeyAction action) => _pMoveEast = action == InputDriver.KeyAction.Pressed;
    private void MoveNorth(in InputDriver.KeyAction action) => _pMoveNorth = action == InputDriver.KeyAction.Pressed;
    private void MoveSouth(in InputDriver.KeyAction action) => _pMoveSouth = action == InputDriver.KeyAction.Pressed;
    private void ZoomIn(in InputDriver.KeyAction action) => 
        PressFilter(action, () => OnZoomEvent(ZoomEventType.Up));
    private void ZoomOut(in InputDriver.KeyAction action) => 
        PressFilter(action, () => OnZoomEvent(ZoomEventType.Down));
    private void ZoomRst(in InputDriver.KeyAction action) => 
        PressFilter(action, () => OnZoomEvent(ZoomEventType.Reset));
    private void NavUp(in InputDriver.KeyAction action) => 
        PressFilter(action, () => OnNavEvent(EventHelper.NavigationEvents.Up));
    private void NavDown(in InputDriver.KeyAction action) => 
        PressFilter(action, () => OnNavEvent(EventHelper.NavigationEvents.Down));
    private void NavSelect(in InputDriver.KeyAction action) =>
        PressFilter(action, () => OnNavEvent(EventHelper.NavigationEvents.Select));
    private void NavEsc(in InputDriver.KeyAction action) => 
        PressFilter(action, () => OnNavEvent(EventHelper.NavigationEvents.Escape));
    
    private static void PressFilter(InputDriver.KeyAction action, Action callback)
    {
        if (action == InputDriver.KeyAction.Pressed) callback.Invoke();
    }
    //Adding item use here.
    // Trying to model this off of how player movement is handled.
    private void UseSlotItem1(in InputDriver.KeyAction action) => PressFilter(action, () => OnItemEvent(0)); //OnItemEvent(0);
    private void UseSlotItem2(in InputDriver.KeyAction action) => PressFilter(action, () => OnItemEvent(1));
    private void UseSlotItem3(in InputDriver.KeyAction action) => PressFilter(action, () => OnItemEvent(2));
    private void UseSlotItem4(in InputDriver.KeyAction action) => PressFilter(action, () => OnItemEvent(3));
    private void UseSlotItem5(in InputDriver.KeyAction action) => PressFilter(action, () => OnItemEvent(4));

    #endregion
}