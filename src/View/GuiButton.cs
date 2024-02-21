using gamespace.Managers;
using Microsoft.Xna.Framework;

namespace gamespace.View;

public class GuiButton : GuiPanel
{
    public delegate void ButtonCallback();
    
    private static Rectangle _drawBox = new Rectangle();
    private ButtonCallback _callback;

    public GuiButton(string title, ButtonCallback callback, GuiManager manager) : base(title, _drawBox, manager)
    {
        _callback = callback;
    }

    public void onPress()
    {
        _callback.Invoke();
    }
    
}