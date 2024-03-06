using System;
using gamespace.Managers;
using gamespace.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace gamespace.View;

public class StatPanel : GuiPanel
{
    private int _health = 100;
    private int _energy = 100;
    
    public StatPanel(Rectangle drawBox, GuiManager manager, GuiPanel parent = null,
        Texture2D background = null) : base("stats", drawBox, manager, parent, background)
    {
    }

    public void HandlePlayerStateEvent(in Player.PlayerState args)
    {
        _health = args.Health;
        _energy = args.Energy;
    }

    public override void Draw(in SpriteBatch batch)
    {
        var boxHeight = DrawBox.Height / 2;
        var hWidth = (int)Math.Round(DrawBox.Width * (_health / 100f));
        var eWidth = (int)Math.Round(DrawBox.Width * (_energy / 100f));
        var hBox = new Rectangle(DrawBox.X + 2, DrawBox.Y - 4, hWidth, boxHeight);
        var eBox = new Rectangle(DrawBox.X + 2, DrawBox.Y + boxHeight - 2, eWidth, boxHeight);
        base.Draw(batch);
        batch.Draw(Background, hBox, Color.Red);
        batch.Draw(Background, eBox, Color.Cyan);
    }
    
}