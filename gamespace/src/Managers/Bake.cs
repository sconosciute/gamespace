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
        menu.AddButton(new GuiButton("Save Game", ButtonCallbacks.SaveGame, menu, manager));
        menu.AddButton(new GuiButton("Load Game", ButtonCallbacks.LoadGame, menu, manager));
        menu.AddButton(new GuiButton("Close Menu", ButtonCallbacks.CloseParentMenu, menu, manager));
        menu.AddButton(new GuiButton("Exit Game", ButtonCallbacks.ExitGame, menu, manager));

        return menu;
    }

    public static StatPanel StatPanel(GraphicsDevice gfx, GuiManager manager)
    {
        var sWidth = gfx.PresentationParameters.Bounds.Width;
        var width = sWidth / 5;
        var height = width / 4;

        var drawBox = new Rectangle(sWidth - width, 0, width, height);
        var stats = new StatPanel(drawBox, manager)
        {
            Shown = true
        };

        return stats;
    }

    private struct ButtonCallbacks
    {
        public static void CloseParentMenu(in GuiPanel parent, in GuiManager manager) => parent.Delete();
        public static void ExitGame(in GuiPanel parent, in GuiManager manager) => manager.ExitGame();
        public static void SaveGame(in GuiPanel parent, in GuiManager manager) => manager.SaveGame();
        public static void LoadGame(in GuiPanel parent, in GuiManager manager) => manager.LoadGame();
    }
}