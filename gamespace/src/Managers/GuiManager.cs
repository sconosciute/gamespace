using System.Collections.Generic;
using gamespace.Model;
using gamespace.View;
using Microsoft.Xna.Framework.Graphics;

namespace gamespace.Managers;

public class GuiManager
{
    private readonly List<GuiPanel> _panels = new();
    private readonly GraphicsDevice _gfx;
    private readonly GameManager _gm;
    private readonly InputManager _input;

    public Texture2D OpaqueBg { get; private set; }
    public Texture2D TransparentBg { get; private set; }

    private readonly SpriteBatch _guiSpriteBatch;

    public GuiManager(in GraphicsDevice gfx, in GameManager gm, in Camera camera)
    {
        _gfx = gfx;
        _gm = gm;
        _input = InputManager.GetInputManager();
        _input.ZoomEvent += camera.HandleZoomEvent;
        _input.InputEvent += HandleInputEvent;
        InputDriver.KeyboardEvent += _input.HandleKeyboardEvent;
        
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
    public void RegisterControlledEntity(in Player player)
    {
        _input.MoveEvent += player.HandleMoveEvent;
    }

    private void RegisterInputHandler(in GuiPanel panel)
    {
        _input.InputEvent += panel.HandleInputEvent;
    }

    public void Update()
    {
        _input.Update();
    }
    
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

    private void HandleInputEvent(in InputManager.NavigationEvents nav)
    {
        if (nav == InputManager.NavigationEvents.Escape)
        {
            OpenMainMenu();
        }
    }

    public void ExitGame() => _gm.ExitGame();
    public void SaveGame() => _gm.SaveGame();
    public void LoadGame() => _gm.LoadGame();

    #region Panels
    /// <summary>
    /// Removes the specified panel from the GUI tree to allow it to be GC'd
    /// </summary>
    /// <param name="panel"></param>
    public void Delete(GuiPanel panel)
    {
        _input.InputEvent -= panel.HandleInputEvent;
        _input.InputEvent += HandleInputEvent;
        _panels.Remove(panel);
    }
    
    private void OpenMainMenu()
    {
        var menu = Bake.MainMenu(_gfx, this);
        _input.InputEvent -= HandleInputEvent;
        RegisterInputHandler(menu);
        _panels.Add(menu);
    }
    
    #endregion
}