using Microsoft.Xna.Framework.Input;


namespace gamespace;

public static class InputDriver
{
    private static KeyboardState CurrentState;
    private static KeyboardState LastState;
    
    public delegate void KeyboardEventHandler(in KeyEvent args);

    public static event KeyboardEventHandler KeyboardEvent;

    private static void OnKeyboardEvent(in KeyEvent args)
    {
        KeyboardEvent?.Invoke(args);
    }

    public static void Update()
    {
        PollKeyboard();
    }

    private static void PollKeyboard()
    {
        LastState = CurrentState;
        CurrentState = Keyboard.GetState();

        foreach (var key in CurrentState.GetPressedKeys())
        {
            if (LastState[key] != KeyState.Down)
            {
                OnKeyboardEvent(new KeyEvent(key, KeyAction.Pressed));
            }
        }

        foreach (var key in LastState.GetPressedKeys())
        {
            if (CurrentState[key] != KeyState.Down)
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
        public Keys Key { get; init; }
        public KeyAction Action { get; init; }

        public KeyEvent(Keys key, KeyAction action)
        {
            Key = key;
            Action = action;
        }
    }

}

