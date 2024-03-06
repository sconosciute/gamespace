using System;
using System.Collections.Generic;
using System.Text.Json;
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

    private readonly Dictionary<Keys, InputCallback> _keyMap;
    private readonly List<(Keys, InputCallback, InputCallbacks)> _defaultBinds = new();
    private readonly Dictionary<InputCallback, InputCallbacks> _callbackToEnum = new();
    private readonly Dictionary<InputCallbacks, InputCallback> _enumToCallback = new();

    private static InputManager _instance;
    private readonly ILogger _log;

    private InputManager()
    {
        _log = Globals.LogFactory.CreateLogger<InputManager>();
        _instance = this;
        InitDefaultBindingsList();
        _keyMap = new Dictionary<Keys, InputCallback>();
        InitBindingsMaps();
        WriteKeyBindConf();

        if (!SettingsManager.TryReadConfig(ConfNames.KeyBinds, out var data)) return;

        _log.LogInformation("Found custom key binds, loading...");
        var keyMap = JsonSerializer.Deserialize<Dictionary<Keys, InputCallbacks>>(data);
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
        _defaultBinds.Add((Keys.W, MoveNorth, InputCallbacks.MoveUp));
        _defaultBinds.Add((Keys.S, MoveSouth, InputCallbacks.MoveDown));
        _defaultBinds.Add((Keys.A, MoveWest, InputCallbacks.MoveLeft));
        _defaultBinds.Add((Keys.D, MoveEast, InputCallbacks.MoveRight));
        _defaultBinds.Add((Keys.Add, ZoomIn, InputCallbacks.ZoomIn));
        _defaultBinds.Add((Keys.Subtract, ZoomOut, InputCallbacks.ZoomOut));
        _defaultBinds.Add((Keys.OemPlus, ZoomRst, InputCallbacks.ZoomRst));
        _defaultBinds.Add((Keys.Up, NavUp, InputCallbacks.NavUp));
        _defaultBinds.Add((Keys.Down, NavDown, InputCallbacks.NavDown));
        _defaultBinds.Add((Keys.Enter, NavSelect, InputCallbacks.NavSelect));
        _defaultBinds.Add((Keys.Escape, NavEsc, InputCallbacks.NavEsc));
    }

    #endregion

    public void SetKeyBind(in Keys key, in InputCallbacks callback)
    {
        var func = _enumToCallback[callback];
        _keyMap.Remove(key);
        _keyMap.Add(key, func);
        WriteKeyBindConf();
    }

    private void WriteKeyBindConf()
    {
        var bindings = new Dictionary<Keys, InputCallbacks>();
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
    /// Player move event handler type.
    /// </summary>
    public delegate void MoveInputHandler(in Vector2 moveVec);

    /// <summary>
    /// Player move event dispatch.
    /// </summary>
    public event MoveInputHandler MoveEvent;

    private void OnMoveEvent(in Vector2 moveVec)
    {
        MoveEvent?.Invoke(moveVec);
    }

    /// <summary>
    /// Zoom event handler type.
    /// </summary>
    public delegate void ZoomEventHandler(ZoomEventType zm);

    /// <summary>
    /// Zoom event dispatch.
    /// </summary>
    public event ZoomEventHandler ZoomEvent;

    private void OnZoomEvent(ZoomEventType zm)
    {
        ZoomEvent?.Invoke(zm);
    }

    public delegate void InputEventHandler(NavigationEvents nav);

    public event InputEventHandler InputEvent;

    private void OnNavEvent(in NavigationEvents nav)
    {
        InputEvent?.Invoke(nav);
    }

    public enum NavigationEvents
    {
        Up,
        Down,
        Select,
        Escape
    }

    #endregion

    #region Input Callbacks

    private void MoveWest(InputDriver.KeyAction action) => _pMoveWest = action == InputDriver.KeyAction.Pressed;
    private void MoveEast(InputDriver.KeyAction action) => _pMoveEast = action == InputDriver.KeyAction.Pressed;
    private void MoveNorth(InputDriver.KeyAction action) => _pMoveNorth = action == InputDriver.KeyAction.Pressed;
    private void MoveSouth(InputDriver.KeyAction action) => _pMoveSouth = action == InputDriver.KeyAction.Pressed;
    private void ZoomIn(InputDriver.KeyAction action) => 
        PressFilter(action, () => OnZoomEvent(ZoomEventType.Up));
    private void ZoomOut(InputDriver.KeyAction action) => 
        PressFilter(action, () => OnZoomEvent(ZoomEventType.Down));
    private void ZoomRst(InputDriver.KeyAction action) => 
        PressFilter(action, () => OnZoomEvent(ZoomEventType.Reset));
    private void NavUp(InputDriver.KeyAction action) => 
        PressFilter(action, () => OnNavEvent(NavigationEvents.Up));
    private void NavDown(InputDriver.KeyAction action) => 
        PressFilter(action, () => OnNavEvent(NavigationEvents.Down));
    private void NavSelect(InputDriver.KeyAction action) =>
        PressFilter(action, () => OnNavEvent(NavigationEvents.Select));
    private void NavEsc(InputDriver.KeyAction action) => 
        PressFilter(action, () => OnNavEvent(NavigationEvents.Escape));
    
    private static void PressFilter(InputDriver.KeyAction action, Action callback)
    {
        if (action == InputDriver.KeyAction.Pressed) callback.Invoke();
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

    #endregion

    private delegate void InputCallback(InputDriver.KeyAction action);
}