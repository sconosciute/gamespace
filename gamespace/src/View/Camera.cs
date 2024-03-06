using System;
using System.Collections.Generic;
using gamespace.Model;
using Loyc;
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

    private float _zoom = 1.2f;
    
    private const float ZoomAdj = 0.01f;

    private Vector2 _trackedPosition = Vector2.Zero;
    
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

        UpdateTranslationMatrix();
    }
    
    //=== EVENT DISPATCH ===--------------------------------------------------------------------------------------------
    public delegate void CameraEventHandler(Matrix scale);

    public event CameraEventHandler CameraEvent;

    protected virtual void OnCameraEvent(Matrix scale)
    {
        CameraEvent?.Invoke(scale);
    }
    
    //=== RENDERING ===-------------------------------------------------------------------------------------------------

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
        _gfx.Clear(Color.Black);
        Globals.SpriteBatch.Begin(transformMatrix: Translation, blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
    }
    
    /// <summary>
    /// Renders the most recently drawn frame to the graphics device and releases the Graphics RenderTarget.
    /// </summary>
    public void RenderFrame()
    {
        Globals.SpriteBatch.End();
    }

    private void UpdateTranslationMatrix()
    {
        var zm = (float) Math.Pow(2, _zoom) - 1;
        const int halfPlayerSize = 8;
        var dx = (_gfx.PresentationParameters.Bounds.Width / 2f) - (_trackedPosition.X * Globals.TileSize + halfPlayerSize) * Globals.Scale * zm;
        var dy = (_gfx.PresentationParameters.Bounds.Height / 2f) - (_trackedPosition.Y * Globals.TileSize  + halfPlayerSize) * Globals.Scale * zm;

        var newTranslation = Matrix.CreateTranslation(dx, dy, 0);
        var scale = Matrix.CreateScale(Globals.Scale * zm);
        Translation = scale * newTranslation;
        OnCameraEvent(scale);
    }

    /// <summary>
    /// Register a RenderObject to be drawn by this camera.
    /// </summary>
    /// <param name="renderObject">Object to draw on screen.</param>
    public void RegisterRenderable(RenderObject renderObject)
    {
        _renderables.Add(renderObject);
    }
    
    //=== EVENT HANDLING ===--------------------------------------------------------------------------------------

    /// <summary>
    /// Listens for Player position updates to update the Camera position.
    /// </summary>
    /// <param name="sender">The sending entity that you wish to track.</param>
    /// <param name="args">Necessary information for this event.</param>
    public void HandleEntityEvent(Guid sender, EntityEventArgs args)
    {
        if (sender != _playerId) return;
        if (args.EventTopic != EntityEventType.Moved) return;
        _trackedPosition = args.NewPosition;
        UpdateTranslationMatrix();
    }
    
    /// <summary>
    /// Adjusts the camera zoom value based on thrown event from InputManager.
    /// </summary>
    public void HandleZoomEvent(in ZoomEventType zm)
    {
        switch (zm)
        {
            case ZoomEventType.Down:
                _zoom -= ZoomAdj;
                break;
            case ZoomEventType.Up:
                _zoom += ZoomAdj;
                break;
            case ZoomEventType.Reset:
                _zoom = 1.2f;
                break;
            default:
                throw new InvalidStateException("Invalid Zoom event in Camera");
        }

        _zoom = Math.Clamp(_zoom, 1f, 1.5f);
        UpdateTranslationMatrix();
        _log.LogDebug("updated zoom to {zm}", _zoom);
    }
}

public enum RenderMode
{
    Immediate,
    Deferred
}

public enum ZoomEventType
{
    Up,
    Down,
    Reset
}