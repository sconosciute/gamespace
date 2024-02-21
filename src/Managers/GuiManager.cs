using System.Collections.Generic;
using gamespace.View;
using Microsoft.Xna.Framework.Graphics;

namespace gamespace.Managers;

public class GuiManager
{
    private List<GuiPanel> _panels = new();

    public Texture2D OpaqueBg { get; init; }
    public Texture2D TransparentBg { get; init; }
    
}