using gamespace.Managers;
using Microsoft.Xna.Framework;

namespace gamespace.View;

public class GuiButton : GuiPanel
{
    public delegate void ButtonCallback();
    
    private static Rectangle _drawBox = new Rectangle();
    private ButtonCallback _callback;

    public GuiButton(string title, ButtonCallback callback, GuiPanel parent, GuiManager manager) : base(title, _drawBox, manager)
    {
        _callback = callback;
    }

    public void UpdateDrawBox(Point position)
    {
        var wh = new Point(Parent.DrawBox.Width * (3 / 4), Parent.DrawBox.Height * (1 / 8));
        DrawBox = new Rectangle(position, wh);
    }

    public void onPress()
    {
        _callback.Invoke();
    }
    
}