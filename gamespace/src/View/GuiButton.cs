using System;
using gamespace.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace gamespace.View;

public class GuiButton : GuiPanel
{
    public delegate void ButtonCallback(GuiPanel parent);
    
    private static readonly Rectangle DefaultBox = new();
    private ButtonCallback _callback;
    public bool Selected { get; set; }

    public GuiButton(string title, ButtonCallback callback, GuiPanel parent, GuiManager manager) 
        : base(title, DefaultBox, manager, parent, manager.TransparentBg)
    {
        _callback = callback;
    }

    public override void Draw(in SpriteBatch batch)
    {
        var color = Selected ? Color.Goldenrod : Color.Aqua;
        batch.Draw(Background, DrawBox, color);
        DrawText(new Vector2(DrawBox.X, DrawBox.Y), Title, batch, true);
    }

    public void UpdateDrawBox(Point position)
    {
        var wAdj = 3f / 4f;
        var hAdj = 1f / 11f;
        var wh = new Point((int)Math.Round(Parent.DrawBox.Width * wAdj), (int)Math.Round(Parent.DrawBox.Height * hAdj));
        DrawBox = new Rectangle(position, wh);
    }

    public void OnPress()
    {
        _callback.Invoke(Parent);
    }
    
}