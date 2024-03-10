using gamespace.Model;
using gamespace.Util;
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
        var drawBox = GetStandardMenuBox(gfx);
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
        var height = width / 3;

        var drawBox = new Rectangle(sWidth - width, 0, width, height);
        var stats = new StatPanel(drawBox, manager)
        {
            Shown = true
        };

        return stats;
    }

    public static MenuPanel InventoryPanel(GraphicsDevice gfx, GuiManager manager, Item[] inventory)
    {
        var drawBox = GetStandardMenuBox(gfx);
        var inv = new MenuPanel(drawBox, manager, "Inventory")
        {
            Shown = true
        };
        for (int item = 0; item < inventory.Length; item++)
        {
            var payload = new EventHelper.PlayerPayload()
            {
                ItemIndex = item
            };
        }

        return inv;
    }

    private static Rectangle GetStandardMenuBox(GraphicsDevice gfx)
    {
        var screenSpace = gfx.PresentationParameters.Bounds;
        var width = (int)(screenSpace.Width * 1f / 3f);
        var topBotBuff = (int)(screenSpace.Height * 1f / 10f);
        var height = screenSpace.Height - (2 * topBotBuff);
        return new Rectangle(width, topBotBuff, width, height);
    }

    private struct ButtonCallbacks
    {
        public static void CloseParentMenu(in GuiPanel parent, in GuiManager manager) => parent.Delete();
        public static void ExitGame(in GuiPanel parent, in GuiManager manager) => manager.ExitGame();
        public static void SaveGame(in GuiPanel parent, in GuiManager manager) => manager.SaveGame();
        public static void LoadGame(in GuiPanel parent, in GuiManager manager) => manager.LoadGame();
        public static void FirePlayerCommand(in EventHelper.PlayerCommand cmd, in EventHelper.PlayerPayload payload,
            in GuiManager manager) => manager.OnPlayerCommandEvent(cmd, payload);
    }
}