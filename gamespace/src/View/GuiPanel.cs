using gamespace.Managers;
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
    
    private string _title;
    private Texture2D _background;


    protected GuiPanel(string title, Rectangle drawBox, GuiManager manager, GuiPanel parent = null,
        Texture2D background = null)
    {
        _title = title;
        DrawBox = drawBox;
        Manager = manager;
        Parent = parent;
        _background = background ?? manager.OpaqueBg;
    }

    protected void DrawText(Vector2 position, string message, SpriteBatch batch)
    {
        //TODO: Provide difference between Title and Body text.
        var scale = new Vector2(Globals.Scale, Globals.Scale);
        batch.DrawString(Globals.Font, message, position, Color.Red, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
    }

    public virtual void Draw(SpriteBatch batch)
    {
        if (Shown)
        {
            batch.Draw(_background, DrawBox, Color.White);
        }
    }
}