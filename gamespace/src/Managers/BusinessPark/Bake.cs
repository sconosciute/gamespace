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
    /// <summary>
    /// The main menu that will contain save, load, close, and exit buttons.
    /// </summary>
    /// <param name="gfx">Main game graphics device.</param>
    /// <param name="manager">The manager of the game.</param>
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

    /// <summary>
    /// The debug menu that will contain the force scale and close menu buttons.
    /// </summary>
    /// <param name="gfx">Main game graphics device.</param>
    /// <param name="manager">The manager of the game.</param>
    public static GuiPanel DebugMenu(GraphicsDevice gfx, GuiManager manager)
    {
        var drawBox = GetStandardMenuBox(gfx);
        var menu = new MenuPanel(drawBox, manager, "Debug Menu")
        {
            Shown = true,
            IsActive = true
        };
        menu.AddButton(new GuiButton("Force Scale", ButtonCallbacks.ToggleForceScale, menu, manager));
        menu.AddButton(new GuiButton("Close Menu", ButtonCallbacks.CloseParentMenu, menu, manager));

        return menu;
    }

    /// <summary>
    /// The panel that contains the player's health, energy, and key items collected.
    /// </summary>
    /// <param name="gfx">Main game graphics device.</param>
    /// <param name="manager">The manager of the game.</param>
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

    /// <summary>
    /// The panel that shows what is in the player's inventory.
    /// </summary>
    /// <param name="gfx">Main game graphics device.</param>
    /// <param name="manager">The manager of the game.</param>
    /// <param name="inventory">An array of Items that represents the player's inventory.</param>
    public static InventoryPanel Inventory(GraphicsDevice gfx, GuiManager manager, Item[] inventory = null)
    {
        var playerInvSlots = 5;
        var sHeight = gfx.PresentationParameters.Bounds.Height;
        var height = sHeight / 10;
        var width = height * playerInvSlots;
        var sWidth = gfx.PresentationParameters.Bounds.Width;

        var drawBox = new Rectangle(sWidth - width, sHeight - height, width, height);
        var inv = new InventoryPanel(drawBox, manager, inventory: inventory)
        {
            Shown = true
        };
        return inv;
    }

    /// <summary>
    /// Gives the right rectangle for displaying any menu in the center of the screen.
    /// </summary>
    /// <param name="gfx">Main game graphics device.</param>
    private static Rectangle GetStandardMenuBox(GraphicsDevice gfx)
    {
        var screenSpace = gfx.PresentationParameters.Bounds;
        var width = (int)(screenSpace.Width * 1f / 3f);
        var topBotBuff = (int)(screenSpace.Height * 1f / 10f);
        var height = screenSpace.Height - (2 * topBotBuff);
        return new Rectangle(width, topBotBuff, width, height);
    }

    /// <summary>
    /// 
    /// </summary>
    private struct ButtonCallbacks
    {
        /// <summary>
        /// Closes menu.
        /// </summary>
        public static void CloseParentMenu(in GuiPanel parent, in GuiManager manager) => parent.Delete();

        /// <summary>
        /// Closes the game.
        /// </summary>
        public static void ExitGame(in GuiPanel parent, in GuiManager manager) => manager.ExitGame();

        /// <summary>
        /// Saves the game.
        /// </summary>
        public static void SaveGame(in GuiPanel parent, in GuiManager manager) => manager.SaveGame();

        /// <summary>
        /// Loads the game from selected save.
        /// </summary>
        public static void LoadGame(in GuiPanel parent, in GuiManager manager) => manager.LoadGame();

        /// <summary>
        /// Toggles cheat to see further.
        /// </summary>
        public static void ToggleForceScale(in GuiPanel parent, in GuiManager manager) => manager.ForceScale();

        public static void FirePlayerCommand(in EventHelper.PlayerCommand cmd, in EventHelper.PlayerPayload payload,
            in GuiManager manager) => manager.OnPlayerCommandEvent(cmd, payload);
    }
}