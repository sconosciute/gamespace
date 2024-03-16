using System;
using System.Collections.Generic;
using gamespace.Util;
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
    /// <summary>
    /// The game's graphics.
    /// </summary>
    private readonly GraphicsDevice _gfx;

    /// <summary>
    /// The unique player ID.
    /// </summary>
    private readonly Guid _playerId;
    
    /// <summary>
    /// List of renderables within the foreground layer.
    /// </summary>
    private readonly List<RenderObject> _foregroundRenderables = new();
    
    /// <summary>
    /// List of renderables within the midground layer.
    /// </summary>
    private readonly List<RenderObject> _midgroundRenderables = new();
    
    /// <summary>
    /// List of renderables within the background layer.
    /// </summary>
    private readonly List<RenderObject> _backgroundRenderables = new();

    /// <summary>
    /// Debug logger.
    /// </summary>
    private readonly ILogger _log;

    /// <summary>
    /// Camera zoom.
    /// </summary>
    private float _zoom = 1.2f;

    /// <summary>
    /// Camera zoom adjust.
    /// </summary>
    private const float ZoomAdj = 0.01f;

    /// <summary>
    /// Tracked position of the camera.
    /// </summary>
    private Vector2 _trackedPosition = Vector2.Zero;

    /// <summary>
    /// The 2D translation Matrix from world to screen coordinates to be used with SpriteBatch for rendering.
    /// </summary>
    private Matrix Translation { get; set; }

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

    /// <summary>
    /// Camera event handler.
    /// </summary>
    public event EventHelper.CameraEventHandler CameraEvent;

    /// <summary>
    /// Invokes scale event for the camera.
    /// </summary>
    protected virtual void OnCameraEvent(Matrix scale)
    {
        CameraEvent?.Invoke(scale);
    }

    /// <summary>
    /// Unrender event handle type.
    /// </summary>
    public void HandleUnrenderEvent(RenderObject robj)
    {
        UnregisterRenderable(robj);
    }

    //=== RENDERING ===-------------------------------------------------------------------------------------------------

    /// <summary>
    /// Draw the current state of all render objects to a frame. May either render immediately upon completion or defer rendering and make a separate call to RenderFrame.
    /// </summary>
    /// <param name="renderMode">Render immediately or defer to a RenderFrame call. Default to immediate.</param>
    public void DrawFrame(RenderMode renderMode = RenderMode.Immediate)
    {
        foreach (var robj in _backgroundRenderables)
        {
            robj.Draw();
        }
        foreach (var robj in _midgroundRenderables)
        {
            robj.Draw();
        }
        foreach (var robj in _foregroundRenderables)
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
        Globals.SpriteBatch.Begin(transformMatrix: Translation, blendState: BlendState.AlphaBlend,
            samplerState: SamplerState.PointClamp);
    }

    /// <summary>
    /// Renders the most recently drawn frame to the graphics device and releases the Graphics RenderTarget.
    /// </summary>
    public static void RenderFrame()
    {
        Globals.SpriteBatch.End();
    }

    /// <summary>
    /// Updates the translation matrix for the camera.
    /// </summary>
    private void UpdateTranslationMatrix()
    {
        var zm = (float)Math.Pow(2, _zoom) - 1;
        const int halfPlayerSize = 8;
        var dx = (_gfx.PresentationParameters.Bounds.Width / 2f) -
                 (_trackedPosition.X * Globals.TileSize + halfPlayerSize) * Globals.Scale * zm;
        var dy = (_gfx.PresentationParameters.Bounds.Height / 2f) -
                 (_trackedPosition.Y * Globals.TileSize + halfPlayerSize) * Globals.Scale * zm;

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
        switch (renderObject.Layer)
        {
            case Layer.Background:
                _backgroundRenderables.Add(renderObject);
                break;
            case Layer.Midground:
                _midgroundRenderables.Add(renderObject);
                break;
            case Layer.Foreground:
                _foregroundRenderables.Add(renderObject);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    /// <summary>
    /// Unregisters the render object.
    /// </summary>
    /// <param name="renderObject">The render object to unregister.</param>
    private void UnregisterRenderable(RenderObject renderObject)
    {
        switch (renderObject.Layer)
        {
            case Layer.Background:
                for (var i = 0; i < _backgroundRenderables.Count; i++)
                {
                    if (_backgroundRenderables[i].EntityId.Equals(renderObject.EntityId))
                    {
                        _backgroundRenderables.RemoveAt(i);
                    }
                }
                break;
            case Layer.Midground:
                for (var i = 0; i < _midgroundRenderables.Count; i++)
                {
                    if (_midgroundRenderables[i].EntityId.Equals(renderObject.EntityId))
                    {
                        _midgroundRenderables.RemoveAt(i);
                    }
                }
                break;
            case Layer.Foreground:
                for (var i = 0; i < _midgroundRenderables.Count; i++)
                {
                    if (_midgroundRenderables[i].EntityId.Equals(renderObject.EntityId))
                    {
                        _midgroundRenderables.RemoveAt(i);
                    }
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    //=== EVENT HANDLING ===--------------------------------------------------------------------------------------

    /// <summary>
    /// Listens for Player position updates to update the Camera position.
    /// </summary>
    /// <param name="sender">The sending entity that you wish to track.</param>
    /// <param name="args">Necessary information for this event.</param>
    public void HandleEntityEvent(in Guid sender, in EventHelper.EntityEventArgs args)
    {
        if (sender != _playerId) return;
        if (args.EventTopic != EventHelper.EntityEventType.Moved) return;
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

/// <summary>
/// Enums of render modes.
/// </summary>
public enum RenderMode
{
    Immediate,
    Deferred
}

/// <summary>
/// Enums of zoom event types.
/// </summary>
public enum ZoomEventType
{
    Up,
    Down,
    Reset
}