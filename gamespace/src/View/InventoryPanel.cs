using gamespace.Managers;
using gamespace.Model;
using gamespace.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace gamespace.View;

public class InventoryPanel : GuiPanel, IPlayerHandler
{
    /// <summary>
    /// Item array to represent the player's inventory.
    /// </summary>
    private Item[] _inventory = new Item[5];
    
    /// <summary>
    /// Potion texture to be displayed.
    /// </summary>
    private readonly Texture2D _potion;
    
    /// <summary>
    /// Creates a new inventory panel object.
    /// </summary>
    /// <param name="drawBox">The box to draw the panel.</param>
    /// <param name="manager">The GUI manager.</param>
    /// <param name="pot">The potion texture to display.</param>
    /// <param name="parent">The parent GUI panel.</param>
    /// <param name="background">The background of the panel.</param>
    /// <param name="inventory">The player inventory.</param>
    public InventoryPanel(Rectangle drawBox, GuiManager manager, Texture2D pot = null, GuiPanel parent = null,
        Texture2D background = null, Item[] inventory = null) : base("Inventory", drawBox, manager, parent, background)
    {
        if (inventory is { Length: < 5 })
        {
            _inventory = inventory;
        }

        _potion = pot ?? (_potion = manager.PotionImg);
    }

    /// <summary>
    /// Player state handler event type.
    /// </summary>
    public void HandlePlayerStateEvent(in EventHelper.PlayerState args)
    {
        _inventory = args.Inventory;
    }

    /// <summary>
    /// Draw the inventory panel.
    /// </summary>
    public override void Draw(in SpriteBatch batch)
    {
        base.Draw(in batch);
        var offset = DrawBox.Width / 5;
        for (var xLoc = 0; xLoc < _inventory.Length ; xLoc++)
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