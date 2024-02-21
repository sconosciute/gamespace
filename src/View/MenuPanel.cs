using System.Collections.Generic;
using gamespace.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace gamespace.View;

public class MenuPanel : GuiPanel
{
    private const string Title = "Main Menu";
    private const int MenuWidth = 360;
    private const int MenuHeight = 640;

    private List<GuiButton> _buttons;
    
    private MenuPanel(Rectangle drawBox, GuiManager manager, List<GuiButton> buttons = null) : base(Title, drawBox, manager)
    {
        if (buttons != null)
        {
            _buttons = buttons;
        }
        
    }

    /// <summary>
    /// Build a new MenuPanel object.
    /// </summary>
    /// <param name="center">The point to center the menu on.</param>
    /// <param name="manager">The GuiManager which the new menu will belong to.</param>
    /// <returns>A new MenuObject centered on the given point.</returns>
    public static MenuPanel BuildMenu(Point center, GuiManager manager)
    {
        var box = GetDrawBox(center);
        return new MenuPanel(box, manager);
    }

    /// <summary>
    /// Builds a new MenuPanel object with the included list of buttons.
    /// </summary>
    /// <param name="center">The point to center the menu on.</param>
    /// <param name="buttons">A list of buttons to add to this menu.</param>
    /// <param name="manager">The GuiManager which the new menu will belong to.</param>
    /// <returns></returns>
    public static MenuPanel BuildMenu(Point center, List<GuiButton> buttons, GuiManager manager)
    {
        var box = GetDrawBox(center);
        return new MenuPanel(box, manager, buttons);
    }

    private static Rectangle GetDrawBox(Point center)
    {
        return new Rectangle(center.X - MenuWidth / 2, center.Y - MenuHeight / 2, MenuWidth, MenuHeight);
    }

    public void AddButton(GuiButton button)
    {
        _buttons.Add(button);
    }
}