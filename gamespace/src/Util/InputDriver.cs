using Microsoft.Xna.Framework.Input;

namespace gamespace.Util;

public static class InputDriver
{
    /// <summary>
    /// The current state of the keyboard.
    /// </summary>
    private static KeyboardState _currentState;
    
    /// <summary>
    /// The last state of the keyboard.
    /// </summary>
    private static KeyboardState _lastState;
    
    /// <summary>
    /// Keyboard event handler type.
    /// </summary>
    /// <param name="args">The key argument.</param>
    public delegate void KeyboardEventHandler(in KeyEvent args);

    /// <summary>
    /// Keyboard event.
    /// </summary>
    public static event KeyboardEventHandler KeyboardEvent;

    /// <summary>
    /// Invokes keyboard event.
    /// </summary>
    /// <param name="args">The key argument.</param>
    private static void OnKeyboardEvent(in KeyEvent args)
    {
        KeyboardEvent?.Invoke(args);
    }

    /// <summary>
    /// Fixed update for keyboard.
    /// </summary>
    public static void Update()
    {
        PollKeyboard();
    }

    /// <summary>
    /// Updates keyboard state.
    /// </summary>
    private static void PollKeyboard()
    {
        _lastState = _currentState;
        _currentState = Keyboard.GetState();

        foreach (var key in _currentState.GetPressedKeys())
        {
            if (_lastState[key] != KeyState.Down)
            {
                OnKeyboardEvent(new KeyEvent(key, KeyAction.Pressed));
            }
        }

        foreach (var key in _lastState.GetPressedKeys())
        {
            if (_currentState[key] != KeyState.Down)
            {
                OnKeyboardEvent(new KeyEvent(key, KeyAction.Released));
            }
        }
    }
    
    public enum KeyAction
    {
        Pressed,
        Released
    }
    
    public class KeyEvent
    {
        /// <summary>
        /// Retrieves the key pressed.
        /// </summary>
        public Keys Key { get; init; }
        
        /// <summary>
        /// Retrieves the action the key is tied to.
        /// </summary>
        public KeyAction Action { get; init; }

        /// <summary>
        /// Connects a keyboard key with an action.
        /// </summary>
        public KeyEvent(Keys key, KeyAction action)
        {
            Key = key;
            Action = action;
        }
    }

    /// <summary>
    /// Dummy equals method.
    /// </summary>
    public static void DummyEquals()
    {
        var args = new KeyEvent(Keys.OemPlus, KeyAction.Pressed);
        OnKeyboardEvent(args);
    }

}

