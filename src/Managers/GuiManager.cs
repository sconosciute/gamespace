using System.Collections.Generic;
using gamespace.Model;
using gamespace.View;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace gamespace.Managers;

public class GuiManager
{
    private readonly List<GuiPanel> _panels = new();
    private readonly GraphicsDevice _gfx;
    private readonly GameManager _gm;
    private readonly InputManager _input;
    private Matrix _drawScale;

    public Texture2D OpaqueBg { get; private set; }
    public Texture2D TransparentBg { get; private set; }

    private readonly SpriteBatch _guiSpriteBatch;

    public GuiManager(GraphicsDevice gfx, GameManager gm, Camera camera)
    {
        _gfx = gfx;
        _gm = gm;
        _input = InputManager.GetInputManager(this);
        _input.ZoomEvent += camera.HandleZoomEvent;
        
        _guiSpriteBatch = new SpriteBatch(_gfx);
    }

    public void InitBgTextures()
    {
        OpaqueBg = _gm.GetTexture(Textures.OpaqueBg);
        TransparentBg = _gm.GetTexture(Textures.TransparentBg);
    }

    /// <summary>
    /// Registers an entity to listen for MoveEvents from the InputManager.
    /// </summary>
    /// <param name="player">The player entity to control, will replace the current entity if one exists.</param>
    public void RegisterControlledEntity(Player player)
    {
        _input.MoveEvent += player.HandleMoveEvent;
    }

    public void Update()
    {
        _input.Update();
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