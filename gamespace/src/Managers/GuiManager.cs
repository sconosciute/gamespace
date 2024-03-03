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
        camera.CameraEvent += HandleCameraEvent;
        
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
        _input.Update(_gm.GameIsPaused);
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
    private void HandleCameraEvent(Matrix scale)
    {
        _drawScale = scale;
    }

    //=== PRE-BAKED PANELS ===------------------------------------------------------------------------------------------
    /// <summary>
    /// Removes the specified panel from the GUI tree to allow it to be GC'd
    /// </summary>
    /// <param name="panel"></param>
    public void Delete(GuiPanel panel)
    {
        _panels.Remove(panel);
    }
    
    public void OpenMainMenu()
    {
        _panels.Add(Bake.MainMenu(_gfx, this));
    }
}