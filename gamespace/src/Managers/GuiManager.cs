using System.Collections.Generic;
using gamespace.Model;
using gamespace.Util;
using gamespace.View;
using Microsoft.Xna.Framework.Graphics;

namespace gamespace.Managers;

public class GuiManager
{
    private readonly List<GuiPanel> _panels = new();
    private readonly GraphicsDevice _gfx;
    private readonly GameManager _gm;
    private readonly InputManager _input;

    private EventHelper.PlayerState _lastState;
    private StatPanel _stats;

    public Texture2D OpaqueBg { get; private set; }
    public Texture2D TransparentBg { get; private set; }

    private readonly SpriteBatch _guiSpriteBatch;

    public GuiManager(in GraphicsDevice gfx, in GameManager gm, in Camera camera)
    {
        _gfx = gfx;
        _gm = gm;
        _gm.RegisterPlayerListener(HandlePlayerStateEvent);
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

    #region Game Loop
    public void Update()
    {
        _input.Update();
    }

    public void RenderGui()
    {
        _guiSpriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
        foreach (var panel in _panels)
        {
            panel.Draw(_guiSpriteBatch);
        }

        _guiSpriteBatch.End();
    }
    
    #endregion
    
    #region Event Handling

    public void HandlePlayerStateEvent(in EventHelper.PlayerState args)
    {
        _lastState = args;
    }
    
    /// <summary>
    /// Registers an entity to listen for MoveEvents from the InputManager.
    /// </summary>
    /// <param name="player">The player entity to control, will replace the current entity if one exists.</param>
    public void RegisterControlledEntity(in Player player)
    {
        _input.MoveEvent += player.HandleMoveEvent;
        _input.ItemUseEvent += player.HandleItemUseEvent;
    }

    private void RegisterInputHandler(in GuiPanel panel)
    {
        _input.InputEvent -= HandleInputEvent;
        _input.InputEvent += panel.HandleInputEvent;
    }

    private void HandleInputEvent(in EventHelper.NavigationEvents nav)
    {
        if (nav == EventHelper.NavigationEvents.Escape)
        {
            OpenMainMenu();
        }
    }

    public void ExitGame() => _gm.ExitGame();
    public void SaveGame() => _gm.SaveGame();
    public void LoadGame() => _gm.LoadGame();
    public void ResumeGame() => _gm.ResumeGame();
    
    #endregion

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
        RegisterInputHandler(menu);
        _panels.Add(menu);
        _gm.PauseGame();
    }

    public void OpenStatPanel()
    {
        if (_stats != null) return;
        _stats = Bake.StatPanel(_gfx, this);
        _gm.RegisterPlayerListener(_stats.HandlePlayerStateEvent);
        _panels.Add(_stats);
    }

    #endregion
}