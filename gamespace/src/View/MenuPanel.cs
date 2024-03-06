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
    private int _selectedButtonIndex;

    public MenuPanel(Rectangle drawBox, GuiManager manager,
        string title, GuiPanel parent = null, List<GuiButton> buttons = null)
        : base(title, drawBox, manager, parent)
    {
        if (buttons != null)
        {
            _buttons = buttons;
            _selectedButtonIndex = 0;
            _buttons[_selectedButtonIndex].Selected = true;
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
            _selectedButtonIndex = 0;
            _buttons[_selectedButtonIndex].Selected = true;
        }
        else
        {
            throw new InvalidStateException("TOO MANY BUTTON, 7 BUTTON ONLY.");
        }
    }

    public override void HandleInputEvent(InputManager.NavigationEvents nav)
    {
        var oldButtonIndex = _selectedButtonIndex;
        switch (nav)
        {
            case InputManager.NavigationEvents.Up: 
                _selectedButtonIndex = _selectedButtonIndex <= 0 ? 0 : _selectedButtonIndex - 1;
                _buttons[oldButtonIndex].Selected = false;
                _buttons[_selectedButtonIndex].Selected = true;
                break;
            case InputManager.NavigationEvents.Down:
            {
                var maxIndex = _buttons.Count - 1;
                _selectedButtonIndex = _selectedButtonIndex >= maxIndex ? maxIndex : _selectedButtonIndex + 1;
                _buttons[oldButtonIndex].Selected = false;
                _buttons[_selectedButtonIndex].Selected = true;
                break;   
            }
            case InputManager.NavigationEvents.Select : _buttons[_selectedButtonIndex].OnPress();
                break;
            case InputManager.NavigationEvents.Escape : Delete();
                break;
            default: throw new ArgumentException("Navigation event does not exist");
        }
    }

    public override void Draw(SpriteBatch batch)
    {
        //TODO: Make button update event based so it doesn't happen every loop.
        var buttonOffset = (int)Math.Round(DrawBox.Height / 10f);
        for (var button = 0; button < _buttons.Count; button++)
        {
            _buttons[button].UpdateDrawBox(new Point(
                DrawBox.X + (int)Math.Round(DrawBox.Width * (1 / 8f)),
                DrawBox.Y + (buttonOffset * (button + 1)))
            );
        }
        
        base.Draw(batch);
        DrawText(new Vector2(DrawBox.X, DrawBox.Y), Title, batch, true);
        foreach (var button in _buttons)
        {
            button.Draw(batch);
        }
    }
}