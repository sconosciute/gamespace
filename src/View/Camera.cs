using System;
using System.Collections.Generic;
using gamespace.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace gamespace.View;

/// <summary>
/// A resolution independent rendering system which builds a scaled viewport within the window frame and follows the specified player
/// </summary>
public class Camera
{
    private readonly GraphicsDevice _gfx;
    private Rectangle _drawDestination;
    
    private readonly Guid _playerId;

    private readonly List<RenderObject> _renderables = new();

    private readonly ILogger _log;
    
    /// <summary>
    /// The 2D translation Matrix from world to screen coordinates to be used with SpriteBatch for rendering.
    /// </summary>
    public Matrix Translation { get; private set; }

    /// <summary>
    /// Initiates a new fixed resolution camera that tracks the Player.
    /// 
    /// The camera will control all rendering and drawing, use BeginFrame in place of SpriteBatch.Begin().
    /// </summary>
    /// <param name="playerId">Entity ID of player to follow.</param>
    /// <param name="graphicsDevice">The backend graphics device.</param>
    public Camera(Guid playerId, GraphicsDevice graphicsDevice)
    {
        _log = Globals.LogFactory.CreateLogger<Camera>();
        _gfx = graphicsDevice;
        _playerId = playerId;

        UpdateTranslationMatrix(Vector2.Zero);
    }

    /// <summary>
    /// Draw the current state of all render objects to a frame. May either render immediately upon completion or defer rendering and make a separate call to RenderFrame.
    /// </summary>
    /// <param name="renderMode">Render immediately or defer to a RenderFrame call. Default to immediate.</param>
    public void DrawFrame(RenderMode renderMode = RenderMode.Immediate)
    {
        foreach (var robj in _renderables)
        {
            robj.Draw();
        }

        if (renderMode == RenderMode.Immediate)
        {
            RenderFrame();
        }
    }
    
    /// <summary>
    /// Begins drawing a new frame. This method will take control of Graphics RenderTarget.
    /// </summary>
    public void BeginFrame()
    {
        Globals.SpriteBatch.Begin(transformMatrix: Translation, blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
    }
    
    /// <summary>
    /// Renders the most recently drawn frame to the graphics device and releases the Graphics RenderTarget.
    /// </summary>
    public void RenderFrame()
    {
        Globals.SpriteBatch.End();
        // _gfx.SetRenderTarget(null);
        // _gfx.Clear(Color.Black);
        // Globals.SpriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
        // Globals.SpriteBatch.Draw(_target, _drawDestination, Color.White);
        // Globals.SpriteBatch.End();
    }

    private void UpdateTranslationMatrix(Vector2 position)
    {
        const int halfPlayerSize = 8;
        var dx = (_gfx.PresentationParameters.Bounds.Width / 2f) - (position.X * Globals.TileSize + halfPlayerSize) * Globals.Zoom;
        var dy = (_gfx.PresentationParameters.Bounds.Height / 2f) - (position.Y * Globals.TileSize  + halfPlayerSize) * Globals.Zoom;

        var newTranslation = Matrix.CreateTranslation(dx, dy, 0);
        var scale = Matrix.CreateScale(Globals.Zoom);
        Translation = scale * newTranslation;
    }

    /// <summary>
    /// Register a RenderObject to be drawn by this camera.
    /// </summary>
    /// <param name="renderObject">Object to draw on screen.</param>
    public void RegisterRenderable(RenderObject renderObject)
    {
        _renderables.Add(renderObject);
    }

    /// <summary>
    /// Listens for Player position updates to update the Camera position.
    /// </summary>
    /// <param name="sender">The sending entity that you wish to track.</param>
    /// <param name="args">Necessary information for this event.</param>
    public void HandleEntityEvent(Guid sender, EntityEventArgs args)
    {
        if (sender != _playerId) return;
        if (args.EventTopic != EntityEventType.Moved) return;
        UpdateTranslationMatrix(args.NewPosition);
    }
}

public enum RenderMode
{
    Immediate,
    Deferred
}