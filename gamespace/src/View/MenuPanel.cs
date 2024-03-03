using System;
using System.Collections.Generic;
using gamespace.Managers;
using Loyc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace gamespace.View;

public class MenuPanel : GuiPanel
{
    private List<GuiButton> _buttons;

    public MenuPanel(Rectangle drawBox, GuiManager manager,
        string title, GuiPanel parent = null, List<GuiButton> buttons = null)
        : base(title, drawBox, manager, parent)
    {
        if (buttons != null)
        {
            _buttons = buttons;
        }
        else
        {
            _buttons = new List<GuiButton>();
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
        //TODO: Make button update event based so it doesn't happen every loop.
        var buttonOffset = (int)Math.Round(DrawBox.Height / 16f);
        for (var button = 0; button < _buttons.Count; button++)
        {
            _buttons[button].UpdateDrawBox(new Point(
                DrawBox.X + (DrawBox.Width * (1 / 8)),
                DrawBox.Y + (buttonOffset * (button + 1)))
            );
        }
        
        base.Draw(batch);
        DrawText(new Vector2(DrawBox.X, DrawBox.Y), Title, batch);
        foreach (var button in _buttons)
        {
            button.Draw(batch);
        }
    }
}