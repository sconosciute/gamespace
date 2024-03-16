using System;
using gamespace.Managers;
using gamespace.Util;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace gamespace.View;

public abstract class GuiPanel
{
    /// <summary>
    /// Determines if this panel should be shown to the user.
    /// </summary>
    public bool Shown { get; set; } = false;
    
    /// <summary>
    /// Determines if this panel should capture input.
    /// </summary>
    public bool IsActive { get; set; } = false;
    
    /// <summary>
    /// The parent of this GuiPanel, may be null if this panel is directly controlled by the GuiManager.
    /// </summary>
    protected GuiPanel Parent { get; init; }

    /// <summary>
    /// The manager of the GUI.
    /// </summary>
    public GuiManager Manager { get; init; }
    
    /// <summary>
    /// A rectangle representation of the draw box.
    /// </summary>
    public Rectangle DrawBox { get; protected set; }

    /// <summary>
    /// The GUI background.
    /// </summary>
    protected Texture2D Background { get; init; }

    /// <summary>
    /// The panel title.
    /// </summary>
    protected string Title { get; init; }

    /// <summary>
    /// Debug logger.
    /// </summary>
    private ILogger _log;
    
    /// <summary>
    /// Creates a GUI panel object.
    /// </summary>
    /// <param name="title">The name of the panel.</param>
    /// <param name="drawBox">The box to draw the panel in.</param>
    /// <param name="manager">The GUI manager.</param>
    /// <param name="parent">The parent panel.</param>
    /// <param name="background">The background image of the panel.</param>
    protected GuiPanel(string title, Rectangle drawBox, GuiManager manager, GuiPanel parent = null,
        Texture2D background = null)
    {
        Title = title;
        DrawBox = drawBox;
        Manager = manager;
        Parent = parent;
        Background = background ?? manager.OpaqueBg;

        _log = Globals.LogFactory.CreateLogger<GuiPanel>();
    }

    /// <summary>
    /// Delete this panel from the UI.
    /// </summary>
    public void Delete()
    {
        Manager.ResumeGame();
        Manager.Delete(this);
    }

    /// <summary>
    /// Handles input events fired from the InputManager to interact with menus.
    /// </summary>
    public virtual void HandleInputEvent(in EventHelper.NavigationEvents nav)
    {
        if (nav == EventHelper.NavigationEvents.Escape)
        {
            Delete();
        }
    }

    /// <summary>
    /// Draws text onto the panel.
    /// </summary>
    protected void DrawText(in Vector2 position, in string message, in SpriteBatch batch, in bool isTitle)
    {
        var scalar = isTitle ? 2f : 3f;
        var scale = new Vector2((float)Math.Round(Globals.Scale / scalar));
        batch.DrawString(Globals.Font, message, position, Color.Black, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
    }

    /// <summary>
    /// Draws the panel.
    /// </summary>
    public virtual void Draw(in SpriteBatch batch)
    {
        if (Shown)
        {
            batch.Draw(Background, DrawBox, Color.White);
        }
    }
}