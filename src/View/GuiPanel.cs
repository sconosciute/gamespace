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

    public GuiManager Manager { get; init; }
    
    protected Rectangle DrawBox { get; private set; }
    
    private string _title;
    private Texture2D _background;


    protected GuiPanel(string title, Rectangle drawBox, GuiManager manager, Texture2D background = null)
    {
        _title = title;
        DrawBox = drawBox;
        Manager = manager;
        _background = background ?? manager.OpaqueBg;
    }

    protected void DrawText(Vector2 position, string message, SpriteBatch batch)
    {
        batch.DrawString(Globals.Font, message, position, Color.Red, 0f, Vector2.Zero, new Vector2(1f, 1f), SpriteEffects.None, 0f);
    }

    public virtual void Draw(SpriteBatch batch)
    {
        if (Shown)
        {
            batch.Draw(_background, DrawBox, Color.White);
        }
    }
}