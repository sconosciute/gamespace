using System.Collections.Generic;
using gamespace.Managers;
using Loyc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace gamespace.View;

public class MenuPanel : GuiPanel
{
    private const string Title = "Main Menu";

    private List<GuiButton> _buttons;

    public MenuPanel(Rectangle drawBox, GuiManager manager, GuiPanel parent = null, List<GuiButton> buttons = null)
        : base(Title, drawBox, manager, parent)
    {
        if (buttons != null)
        {
            _buttons = buttons;
        }
    }

    public void AddButton(GuiButton button)
    {
        if (_buttons.Count <= 7)
        {
            _buttons.Add(button);
        }
        else
        {
            throw new InvalidStateException("TOO MANY BUTTON, 7 BUTTON ONLY.");
        }
    }

    public override void Draw(SpriteBatch batch)
    {
        //TODO: Make this a GuiScaleChange event.
        var buttonOffset = DrawBox.Height * (1 / 16);
        for (int button = 0; button < _buttons.Count; button++)
        {
            _buttons[button].UpdateDrawBox(new Point(DrawBox. X + (DrawBox.Width * (1 / 8)) ));
        }
        base.Draw(batch);
        DrawText(new Vector2(DrawBox.X, DrawBox.Y), Title, batch);
    }
}