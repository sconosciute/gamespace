using gamespace.Managers;
using Microsoft.Xna.Framework;

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
    
    private string _title;
    private Rectangle _drawBox;


    protected GuiPanel(string title, Rectangle drawBox, GuiManager manager)
    {
        _title = title;
        _drawBox = drawBox;
        Manager = manager;
    }
}