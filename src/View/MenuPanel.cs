using System.Collections.Generic;
using gamespace.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace gamespace.View;

public class MenuPanel : GuiPanel
{
    private const string Title = "Main Menu";
    private const float MenuWidth = 320;
    private const float MenuHeight = 320;

    private List<GuiButton> _buttons;
    
    public MenuPanel(Rectangle drawBox, GuiManager manager, List<GuiButton> buttons = null) : base(Title, drawBox, manager)
    {
        if (buttons != null)
        {
            _buttons = buttons;
        }
        
    }

    public void AddButton(GuiButton button)
    {
        _buttons.Add(button);
    }

    public override void Draw(SpriteBatch batch)
    {
        base.Draw(batch);
        DrawText(new Vector2(DrawBox.X, DrawBox.Y), Title, batch);
    }
}