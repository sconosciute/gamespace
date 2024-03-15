using gamespace.Managers;
using gamespace.Managers.Database;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace gamespace.View;

public class DbPanel : GuiPanel
{
    public DbPanel(Rectangle drawBox, GuiManager manager, GuiPanel parent = null,
        Texture2D background = null) : base("Statistics", drawBox, manager, parent, background)
    {
    }

    public override void Draw(in SpriteBatch batch)
    {
        base.Draw(in batch);
        
        DrawText(new Vector2(DrawBox.X, DrawBox.Y), Title, batch, true);
        var stats = DbHandler.GetAllStats();
        if (stats == null) return;

        var offset = DrawBox.Height / 10;
        var inset = DrawBox.Width / 8;
        var round = 1;

        foreach (var stat in stats)
        {
            var print = $"{stat.StatName}:    {stat.Value}";
            var pos = new Vector2(DrawBox.X + inset, DrawBox.Y + (offset * round));
            DrawText(pos, print, batch, false);
            round++;
        }
    }
}