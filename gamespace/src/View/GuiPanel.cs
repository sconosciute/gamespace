using System;
using gamespace.Managers;
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
    public GuiPanel Parent { get; init; }

    public GuiManager Manager { get; init; }
    
    public Rectangle DrawBox { get; protected set; }

    protected Texture2D Background { get; init; }

    protected string Title { get; init; }

    private ILogger _log;
    


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
        Manager.Delete(this);
    }

    /// <summary>
    /// Handles input events fired from the InputManager to interact with menus.
    /// </summary>
    public virtual void HandleInputEvent(in InputManager.NavigationEvents nav)
    {
        _log.LogDebug("Tried to ask non-input panel to handle input event.");
    }

    protected void DrawText(in Vector2 position, in string message, in SpriteBatch batch, in bool isTitle)
    {
        var scalar = isTitle ? 2f : 3f;
        var scale = new Vector2((float)Math.Round(Globals.Scale / scalar));
        batch.DrawString(Globals.Font, message, position, Color.Black, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
    }

    public virtual void Draw(in SpriteBatch batch)
    {
        if (Shown)
        {
            batch.Draw(Background, DrawBox, Color.White);
        }
    }
}