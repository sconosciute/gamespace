using System;
using gamespace.Managers;
using gamespace.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace gamespace.View;

public class GuiButton : GuiPanel
{
    /// <summary>
    /// Button callback event.
    /// </summary>
    /// <param name="parent">The panel being interacted with.</param>
    /// <param name="manager">The manager to update.</param>
    public delegate void ButtonCallback(in GuiPanel parent, in GuiManager manager);

    /// <summary>
    /// A rectangle representing the default box for the button.
    /// </summary>
    private static readonly Rectangle DefaultBox = new();
    
    /// <summary>
    /// Button callback.
    /// </summary>
    private readonly ButtonCallback _callback;
    
    /// <summary>
    /// Boolean to see if a button is selected or not.
    /// </summary>
    public bool Selected { get; set; }

    /// <summary>
    /// Creates a new GUI button object.
    /// </summary>
    /// <param name="title">Title of the button.</param>
    /// <param name="callback">The callback event tied to the button.</param>
    /// <param name="parent">The panel the button resides on.</param>
    /// <param name="manager">The manager that updates.</param>
    public GuiButton(string title, ButtonCallback callback, GuiPanel parent, GuiManager manager)
        : base(title, DefaultBox, manager, parent, manager.TransparentBg)
    {
        _callback = callback;
    }

    /// <summary>
    /// Method that draws the button.
    /// </summary>
    public override void Draw(in SpriteBatch batch)
    {
        var color = Selected ? Color.Goldenrod : Color.Aqua;
        batch.Draw(Background, DrawBox, color);
        DrawText(new Vector2(DrawBox.X, DrawBox.Y), Title, batch, true);
    }

    /// <summary>
    /// Updates the rectangle/box the button is drawn on.
    /// </summary>
    /// <param name="position">The position of the button.</param>
    public void UpdateDrawBox(Point position)
    {
        const float wAdj = 3f / 4f;
        const float hAdj = 1f / 11f;
        var wh = new Point((int)Math.Round(Parent.DrawBox.Width * wAdj), (int)Math.Round(Parent.DrawBox.Height * hAdj));
        DrawBox = new Rectangle(position, wh);
    }

    /// <summary>
    /// Invokes an event when the button is pressed.
    /// </summary>
    public void OnPress()
    {
        _callback.Invoke(Parent, Parent.Manager);
    }
}