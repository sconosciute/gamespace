using System;
using gamespace.Managers;
using gamespace.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace gamespace.View;

public class StatPanel : GuiPanel, IPlayerHandler
{
    /// <summary>
    /// The health of the player.
    /// </summary>
    private int _health = 100;
    
    /// <summary>
    /// The energy of the player.
    /// </summary>
    private int _energy = 100;
    
    /// <summary>
    /// The key item count for the player.
    /// </summary>
    private int _keyCount = 0;
    
    /// <summary>
    /// Creates a new stat panel object.
    /// </summary>
    /// <param name="drawBox">The box to draw the stat panel.</param>
    /// <param name="manager">The GUI manager.</param>
    /// <param name="parent">The panel's parent panel.</param>
    /// <param name="background">The background of the panel.</param>
    public StatPanel(Rectangle drawBox, GuiManager manager, GuiPanel parent = null,
        Texture2D background = null) : base("stats", drawBox, manager, parent, background)
    {
    }

    /// <summary>
    /// Player state event handler type.
    /// </summary>
    /// <param name="args">The player state event.</param>
    public void HandlePlayerStateEvent(in EventHelper.PlayerState args)
    {
        _health = args.Health;
        _energy = args.Energy;
        _keyCount = args.KeyItems;
    }

    /// <summary>
    /// Draws the stat panel.
    /// </summary>
    public override void Draw(in SpriteBatch batch)
    {
        const int buf = 2;
        var boxHeight = DrawBox.Height / 3;
        var hWidth = (int)Math.Round(DrawBox.Width * (_health / 100f));
        var eWidth = (int)Math.Round(DrawBox.Width * (_energy / 100f));
        var hBox = new Rectangle(DrawBox.X + 2, DrawBox.Y - (buf * 2), hWidth, boxHeight);
        var eBox = new Rectangle(DrawBox.X + 2, DrawBox.Y + boxHeight - buf, eWidth, boxHeight);
        base.Draw(batch);
        batch.Draw(Background, hBox, Color.Red);
        batch.Draw(Background, eBox, Color.Cyan);
        var drawPos = new Vector2(DrawBox.X + buf, DrawBox.Y + (boxHeight * 2 + buf));
        DrawText(drawPos, $"{_keyCount} / 4", batch, true);
    }
    
}