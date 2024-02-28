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
    public static bool Moving => _direction != Vector2.Zero;

    private delegate void InputCallback();

    private readonly Dictionary<Keys, InputCallback> _keyMap;
    private readonly List<(Keys, InputCallback, InputCallbacks)> _defaultBinds = new();
    private readonly Dictionary<InputCallback, InputCallbacks> _callbackToEnum = new();
    private readonly Dictionary<InputCallbacks, InputCallback> _enumToCallback = new();

    private static InputManager _instance;

    private GuiManager _manager;

    private readonly ILogger _log;

    private InputManager(in GuiManager manager)
    {
        _log = Globals.LogFactory.CreateLogger<InputManager>();
        _manager = manager;
        _instance = this;
        InitDefaultBindingsList();
        _keyMap = new Dictionary<Keys, InputCallback>();
        InitBindingsMaps();

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
        _defaultBinds.Add((Keys.W, MoveUp, InputCallbacks.MoveUp));
        _defaultBinds.Add((Keys.S, MoveDown, InputCallbacks.MoveDown));
        _defaultBinds.Add((Keys.A, MoveLeft, InputCallbacks.MoveLeft));
        _defaultBinds.Add((Keys.D, MoveRight, InputCallbacks.MoveRight));
        _defaultBinds.Add((Keys.Add, ZoomIn, InputCallbacks.ZoomIn));
        _defaultBinds.Add((Keys.Subtract, ZoomOut, InputCallbacks.ZoomOut));
        _defaultBinds.Add((Keys.OemPlus, ZoomRst, InputCallbacks.ZoomRst));
    }

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

    public static InputManager GetInputManager(in GuiManager manager)
    {
        return _instance ??= new InputManager(manager);
    }

    public void Update(in bool gameIsPaused)
    {
        var oldDir = _direction;
        _direction = Vector2.Zero;
        var keyboardState = Keyboard.GetState();

        foreach (var key in keyboardState.GetPressedKeys())
        {
            _keyMap.TryGetValue(key, out var callback);
            callback?.Invoke();
        }

        if (_direction == oldDir) return;
        if (_direction != Vector2.Zero)
        {
            _direction.Normalize();
        }
        OnMoveEvent(_direction);
    }

    //=== EVENT DISPATCH ===--------------------------------------------------------------------------------------------

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

    //=== INPUT CALLBACKS ===-------------------------------------------------------------------------------------------
    private void MoveLeft() => _direction.X--;
    private void MoveRight() => _direction.X++;
    private void MoveUp() => _direction.Y--;
    private void MoveDown() => _direction.Y++;
    private void ZoomIn() => OnZoomEvent(ZoomEventType.Up);
    private void ZoomOut() => OnZoomEvent(ZoomEventType.Down);
    private void ZoomRst() => OnZoomEvent(ZoomEventType.Reset);

    public enum InputCallbacks
    {
        MoveLeft,
        MoveRight,
        MoveUp,
        MoveDown,
        ZoomIn,
        ZoomOut,
        ZoomRst
    }
}