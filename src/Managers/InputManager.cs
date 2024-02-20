using gamespace.View;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace gamespace.Managers;

public static class InputManager
{
    private static Vector2 _direction;
    public static Vector2 Direction => _direction;
    public static bool Moving => _direction != Vector2.Zero;

    public static void Update()
    {
        _direction = Vector2.Zero;
        var keyboardState = Keyboard.GetState();

        if (keyboardState.GetPressedKeyCount() > 0)
        {
            if (keyboardState.IsKeyDown(Keys.A)) _direction.X--;
            if (keyboardState.IsKeyDown(Keys.D)) _direction.X++;
            if (keyboardState.IsKeyDown(Keys.W)) _direction.Y--;
            if (keyboardState.IsKeyDown(Keys.S)) _direction.Y++;
            if (keyboardState.IsKeyDown(Keys.Add)) OnZoomEvent(ZoomEventType.Up);
            if (keyboardState.IsKeyDown(Keys.Subtract)) OnZoomEvent(ZoomEventType.Down);
            if (keyboardState.IsKeyDown(Keys.OemPlus)) OnZoomEvent(ZoomEventType.Reset);
        }

        if (_direction != Vector2.Zero)
        {
            _direction.Normalize();
        }
    }

    private static void OnZoomEvent(ZoomEventType zm)
    {
        ZoomEvent?.Invoke(zm);
    }

    public delegate void ZoomEventHandler(ZoomEventType zm);
    public static event ZoomEventHandler ZoomEvent;
}