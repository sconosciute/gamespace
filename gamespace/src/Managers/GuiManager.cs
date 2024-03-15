using System;
using System.Collections.Generic;
using gamespace.Model;
using gamespace.Util;
using gamespace.View;
using Microsoft.Xna.Framework.Graphics;

namespace gamespace.Managers;

public class GuiManager
{
    /// <summary>
    /// A list of all panels.
    /// </summary>
    private readonly List<GuiPanel> _panels = new();
    
    /// <summary>
    /// The main graphics of the game.
    /// </summary>
    private readonly GraphicsDevice _gfx;
    
    /// <summary>
    /// Game manager object of the game.
    /// </summary>
    private readonly GameManager _gm;
    
    /// <summary>
    /// The input of the player.
    /// </summary>
    private readonly InputManager _input;

    /// <summary>
    /// The last state the player was in.
    /// </summary>
    private EventHelper.PlayerState _lastState;
    
    /// <summary>
    /// The stat panel.
    /// </summary>
    private StatPanel _stats;

    /// <summary>
    /// An opaque background for the GUI.
    /// </summary>
    public Texture2D OpaqueBg { get; private set; }
    /// <summary>
    /// A transparent background for the GUI.
    /// </summary>
    public Texture2D TransparentBg { get; private set; }
    
    /// <summary>
    /// Potion image to display to the player inventory.
    /// </summary>
    public Texture2D PotionImg { get; private set; }
    
    /// <summary>
    /// The drawings of the GUI.
    /// </summary>
    private readonly SpriteBatch _guiSpriteBatch;

    /// <summary>
    /// Generates a GUI manager that initializes and starts up the GUI.
    /// </summary>
    /// <param name="gfx">The main graphics of the game.</param>
    /// <param name="gm">The current game.</param>
    /// <param name="camera">The camera object that will render the GUI</param>
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

    /// <summary>
    /// Initializes background textures used in the GUI.
    /// </summary>
    public void InitBgTextures()
    {
        OpaqueBg = _gm.GetTexture(Textures.OpaqueBg);
        TransparentBg = _gm.GetTexture(Textures.TransparentBg);
        PotionImg = _gm.GetTexture(Textures.PotLarge);
    }

    #region Game Loop
    
    /// <summary>
    /// Updates the input.
    /// </summary>
    public void Update()
    {
        _input.Update();
    }

    /// <summary>
    /// Renders/draws the GUI.
    /// </summary>
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

    /// <summary>
    /// Handles the player's current state.
    /// </summary>
    /// <param name="args">The event of the player.</param>
    private void HandlePlayerStateEvent(in EventHelper.PlayerState args)
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
        _input.PlayerShootEvent += player.OnPlayerShootSender;
        player.shotRecieved += _gm.PlayerShootHandler;
    }

    /// <summary>
    /// Registers inputs.
    /// </summary>
    /// <param name="panel">The texture or complete Atlas to display.</param>
    private void RegisterInputHandler(in GuiPanel panel)
    {
        _input.InputEvent -= HandleInputEvent;
        _input.InputEvent += panel.HandleInputEvent;
    }

    /// <summary>
    /// Handles input to navigate the menu.
    /// </summary>
    /// <param name="nav">The current navigation input.</param>
    private void HandleInputEvent(in EventHelper.NavigationEvents nav)
    {
        switch (nav)
        {
            case EventHelper.NavigationEvents.Escape:
                OpenMainMenu();
                break;
            case EventHelper.NavigationEvents.Debug:
                OpenDebugMenu();
                break;
            case EventHelper.NavigationEvents.Up:
                break;
            case EventHelper.NavigationEvents.Down:
                break;
            case EventHelper.NavigationEvents.Select:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(nav), nav, null);
        }
    }

    /// <summary>
    /// The event for player commands.
    /// </summary>
    public event EventHelper.PlayerCommandHandler PlayerCommandEvent;

    /// <summary>
    /// Invokes event based on the player command.
    /// </summary>
    /// <param name="cmd">The current player command.</param>
    /// <param name="payload">The player payload.</param>
    public void OnPlayerCommandEvent(in EventHelper.PlayerCommand cmd, in EventHelper.PlayerPayload payload)
    {
        PlayerCommandEvent?.Invoke(cmd, payload);
    }

    /// <summary>
    /// Exits the game.
    /// </summary>
    public void ExitGame() => _gm.ExitGame();
    
    /// <summary>
    /// Saves the game.
    /// </summary>
    public void SaveGame() => _gm.SaveGame();
    
    /// <summary>
    /// Loads the game.
    /// </summary>
    public void LoadGame() => _gm.LoadGame();
    
    /// <summary>
    /// Resumes the game.
    /// </summary>
    public void ResumeGame() => _gm.ResumeGame();
    
    #endregion

    #region Panels

    /// <summary>
    /// Removes the specified panel from the GUI tree to allow it to be GC'd
    /// </summary>
    /// <param name="panel">The panel to be deleted.</param>
    public void Delete(GuiPanel panel)
    {
        _input.InputEvent -= panel.HandleInputEvent;
        _input.InputEvent += HandleInputEvent;
        _panels.Remove(panel);
    }

    /// <summary>
    /// Opens the main menu.
    /// </summary>
    private void OpenMainMenu()
    {
        var menu = Bake.MainMenu(_gfx, this);
        RegisterInputHandler(menu);
        _panels.Add(menu);
        _gm.PauseGame();
    }
    
    /// <summary>
    /// Opens debug/cheat menu.
    /// </summary>
    private void OpenDebugMenu()
    {
        var menu = Bake.DebugMenu(_gfx, this);
        RegisterInputHandler(menu);
        _panels.Add(menu);
        _gm.PauseGame();
    }

    /// <summary>
    /// Adds stat and inventory panels.
    /// </summary>
    public void OpenPersistentElements()
    {
        if (_stats != null) return;
        _stats = Bake.StatPanel(_gfx, this);
        var inv = Bake.Inventory(_gfx, this);
        _gm.RegisterPlayerListener(_stats.HandlePlayerStateEvent);
        _gm.RegisterPlayerListener(inv.HandlePlayerStateEvent);
        _panels.Add(_stats);
        _panels.Add(inv);
    }

    /// <summary>
    /// Displays stats of the player.
    /// </summary>
    public void OpenInfo()
    {
        var stat = Bake.Info(_gfx, this);
        RegisterInputHandler(stat);
        _panels.Add(stat);
    }

    #endregion

    /// <summary>
    /// Forces the scale based on if the debug force scale button is activated.
    /// </summary>
    public void ForceScale()
    {
        Globals.DebugForceScale = !Globals.DebugForceScale;
        Globals.UpdateScale(_gfx);
        InputDriver.DummyEquals();
    }
}