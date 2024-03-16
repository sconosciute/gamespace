using System;
using System.Collections.Generic;
using gamespace.Managers;
using gamespace.Util;
using Loyc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace gamespace.View;

public class MenuPanel : GuiPanel
{
    /// <summary>
    /// List of selectable buttons.
    /// </summary>
    private readonly List<GuiButton> _buttons;
    
    /// <summary>
    /// Keeps track of what index a certain button is.
    /// </summary>
    private int _selectedButtonIndex;

    /// <summary>
    /// Creates a new menu panel object.
    /// </summary>
    /// <param name="drawBox">The box to draw the panel in.</param>
    /// <param name="manager">The GUI manager.</param>
    /// <param name="title">The title of the panel.</param>
    /// <param name="parent">The parent of the GUI panel.</param>
    /// <param name="buttons">List of selectable buttons.</param>
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

    /// <summary>
    /// Adds a button to the menu panel.
    /// </summary>
    /// <param name="button">The button to add.</param>
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

    /// <summary>
    /// Input event handler.
    /// </summary>
    /// <param name="nav">Button navigation event.</param>
    public override void HandleInputEvent(in EventHelper.NavigationEvents nav)
    {
        var oldButtonIndex = _selectedButtonIndex;
        switch (nav)
        {
            case EventHelper.NavigationEvents.Up:
                _selectedButtonIndex = _selectedButtonIndex <= 0 ? 0 : _selectedButtonIndex - 1;
                _buttons[oldButtonIndex].Selected = false;
                _buttons[_selectedButtonIndex].Selected = true;
                break;
            case EventHelper.NavigationEvents.Down:
            {
                var maxIndex = _buttons.Count - 1;
                _selectedButtonIndex = _selectedButtonIndex >= maxIndex ? maxIndex : _selectedButtonIndex + 1;
                _buttons[oldButtonIndex].Selected = false;
                _buttons[_selectedButtonIndex].Selected = true;
                break;
            }
            case EventHelper.NavigationEvents.Select:
                _buttons[_selectedButtonIndex].OnPress();
                break;
            case EventHelper.NavigationEvents.Escape:
                Delete();
                break;
            case EventHelper.NavigationEvents.Debug:
            default: throw new ArgumentException("Navigation event does not exist");
        }
    }

    /// <summary>
    /// Draw the menu panel.
    /// </summary>
    /// <param name="batch">The sprite to draw.</param>
    public override void Draw(in SpriteBatch batch)
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