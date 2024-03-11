using gamespace.Managers;
using gamespace.Model;
using gamespace.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace gamespace.View;

public class InventoryPanel : GuiPanel, IPlayerHandler
{
    private Item[] _inventory = new Item[5];
    private Texture2D _potion;
    
    public InventoryPanel(Rectangle drawBox, GuiManager manager, Texture2D pot = null, GuiPanel parent = null,
        Texture2D background = null, Item[] inventory = null) : base("Inventory", drawBox, manager, parent, background)
    {
        if (inventory is { Length: < 5 })
        {
            _inventory = inventory;
        }

        _potion = pot ?? (_potion = manager.PotionImg);
    }

    public void HandlePlayerStateEvent(in EventHelper.PlayerState args)
    {
        _inventory = args.Inventory;
    }

    public override void Draw(in SpriteBatch batch)
    {
        base.Draw(in batch);
        var offset = DrawBox.Width / 5;
        for (int xLoc = 0; xLoc < _inventory.Length ; xLoc++)
        {
            var pos = new Vector2(DrawBox.X + (offset * xLoc), DrawBox.Y);
            if (_inventory[xLoc] != null)
            {
                var box = new Rectangle(DrawBox.X + (offset * xLoc), DrawBox.Y, offset, DrawBox.Height);
                pos.X += 5;
                batch.Draw(_potion, box, Color.White);
            }
            DrawText(pos, $"{xLoc + 1}", batch, true);
            
        }
    }
}