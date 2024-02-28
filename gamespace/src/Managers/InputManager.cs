using System.Collections.Generic;
using System.Text.Json;
using gamespace.View;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace gamespace.Managers;

public sealed class InputManager
{
    private static Vector2 _direction;
    public static Vector2 Direction => _direction;
    public static bool Moving => _direction != Vector2.Zero;

    public delegate void InputCallback();

    private Dictionary<Keys, InputCallback> _keyMap;

    private static InputManager _instance;

    private GuiManager _manager;

    private InputManager(GuiManager manager)
    {
        _manager = manager;
        _instance = this;
        _keyMap = new Dictionary<Keys, InputCallback>();
        _keyMap.Add(Keys.W, MoveUp);
        _keyMap.Add(Keys.S, MoveDown);
        _keyMap.Add(Keys.A, MoveLeft);
        _keyMap.Add(Keys.D, MoveRight);
        _keyMap.Add(Keys.Add, ZoomIn);
        _keyMap.Add(Keys.Subtract, ZoomOut);
        _keyMap.Add(Keys.OemPlus, ZoomRst);
    }

    private void WriteKeyBindConf()
    {
        var json = JsonSerializer.Serialize(_keyMap);
        SettingsManager.TryWriteConfig(ConfNames.KeyBinds, json);
    }

    public static InputManager GetInputManager(GuiManager manager)
    {
        return _instance ??= new InputManager(manager);
    }

    public void Update(in bool gameIsPaused)
    {
        _direction = Vector2.Zero;
        var keyboardState = Keyboard.GetState();

        foreach (var key in keyboardState.GetPressedKeys())
        {
            _keyMap.TryGetValue(key, out var callback);
            callback?.Invoke();
        }
        if (_direction == Vector2.Zero) return;
        _direction.Normalize();
        OnMoveEvent(_direction);
    }
    
    //=== INPUT CALLBACKS ===-------------------------------------------------------------------------------------------
    private void MoveLeft() => _direction.X--;
    private void MoveRight() => _direction.X++;
    private void MoveUp() => _direction.Y--;
    private void MoveDown() => _direction.Y++;
    private void ZoomIn() => OnZoomEvent(ZoomEventType.Up);
    private void ZoomOut() => OnZoomEvent(ZoomEventType.Down);
    private void ZoomRst() => OnZoomEvent(ZoomEventType.Reset);
    
    //=== EVENT DISPATCH ===--------------------------------------------------------------------------------------------
    
    /// <summary>
    /// Player move event handler type.
    /// </summary>
    public delegate void MoveInputHandler(Vector2 moveVec);

   /// <summary>
   /// Player move event dispatch.
   /// </summary>
    public event MoveInputHandler MoveEvent;
    private void OnMoveEvent(Vector2 moveVec)
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
}