using gamespace.View;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace gamespace.Managers;

/// <summary>
/// Utility class for getting a prebaked GUI element.
/// </summary>
public static class Bake
{
    public static GuiPanel MainMenu(GraphicsDevice gfx, GuiManager manager)
    {
        var screenSpace = gfx.PresentationParameters.Bounds;
        var width = (int)(screenSpace.Width * 1f / 3f);
        var topBotBuff = (int)(screenSpace.Height * 1f / 10f);
        var height = screenSpace.Height - (2 * topBotBuff);
        var drawBox = new Rectangle(width, topBotBuff, width, height);
        var menu = new MenuPanel(drawBox, manager, "Main Menu")
        {
            Shown = true,
            IsActive = true
        };
        menu.AddButton(new GuiButton("Close", ButtonCallbacks.CloseParentMenu, menu, manager));

        return menu;

    }

    struct ButtonCallbacks
    {
        public static void CloseParentMenu(GuiPanel parent) => parent.Delete();
    }
}