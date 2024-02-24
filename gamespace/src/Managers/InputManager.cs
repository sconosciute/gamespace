using gamespace.View;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace gamespace.Managers;

public sealed class InputManager
{
    private static Vector2 _direction;
    public static Vector2 Direction => _direction;
    public static bool Moving => _direction != Vector2.Zero;

    private static InputManager _instance;

    private GuiManager _manager;

    private InputManager(GuiManager manager)
    {
        _manager = manager;
        _instance = this;
    }

    public static InputManager GetInputManager(GuiManager manager)
    {
        return _instance ??= new InputManager(manager);
    }

    public void Update()
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

        if (_direction == Vector2.Zero) return;
        _direction.Normalize();
        OnMoveEvent(_direction);
    }
    
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