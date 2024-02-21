using System.Collections.Generic;
using gamespace.View;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace gamespace.Managers;

public class GuiManager
{
    private readonly List<GuiPanel> _panels = new();
    private readonly GraphicsDevice _gfx;
    private readonly GameManager _gm;
    private Matrix _drawScale;

    public Texture2D OpaqueBg { get; init; }
    public Texture2D TransparentBg { get; init; }

    private readonly SpriteBatch _guiSpriteBatch;

    public GuiManager(GraphicsDevice gfx, GameManager gm)
    {
        _gfx = gfx;
        _gm = gm;

        OpaqueBg = _gm.GetTexture(Textures.OpaqueBg);
        TransparentBg = _gm.GetTexture(Textures.TransparentBg);
        _guiSpriteBatch = new SpriteBatch(_gfx);
    }

    //=== GUI RENDERING ===---------------------------------------------------------------------------------------------
    public void RenderGui()
    {
        _guiSpriteBatch.Begin(blendState: BlendState.AlphaBlend,
            samplerState: SamplerState.PointClamp);
        foreach (var panel in _panels)
        {
            panel.Draw(_guiSpriteBatch);
        }

        _guiSpriteBatch.End();
    }

    //=== EVENT HANDLING ===--------------------------------------------------------------------------------------------
    public void HandleCameraEvent(Matrix scale)
    {
        _drawScale = scale;
    }

    //=== PRE-BAKED PANELS ===------------------------------------------------------------------------------------------

    public void OpenMainMenu()
    {
        var center = new Point(_gfx.PresentationParameters.Bounds.Width / 2, _gfx.PresentationParameters.Bounds.Height / 2);
        var menu = MenuPanel.BuildMenu(center, this);
        menu.Shown = true;
        _panels.Add(menu);
    }
}