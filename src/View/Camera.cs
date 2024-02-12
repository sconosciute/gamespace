using System;
using System.Collections.Generic;
using gamespace.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace gamespace.View;

/// <summary>
/// A resolution independent rendering system which builds a scaled viewport within the window frame and follows the specified player
/// </summary>
public class Camera
{
    private readonly RenderTarget2D _target;
    private readonly GraphicsDevice _gfx;
    private Rectangle _drawDestination;
    
    private readonly Guid _playerId;

    private readonly List<RenderObject> _renderables = new();
    
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
    /// <param name="resolution">Point representing target resolution (width, height).</param>
    public Camera(Guid playerId, GraphicsDevice graphicsDevice, Point resolution)
    {
        _gfx = graphicsDevice;
        _target = new RenderTarget2D(_gfx, resolution.X, resolution.Y);
        _playerId = playerId;

        ScaleViewport();
        UpdateTranslationMatrix(Vector2.Zero);
    }

    /// <summary>
    /// Draw the current state of all render objects to a frame. May either render immediately upon completion or defer rendering and make a separate call to RenderFrame.
    /// </summary>
    /// <param name="renderNow">Render immediately upon completion if true, defaults to true.</param>
    public void DrawFrame(bool renderNow = true)
    {
        foreach (var robj in _renderables)
        {
            robj.Draw();
        }

        if (renderNow)
        {
            RenderFrame();
        }
    }
    
    /// <summary>
    /// Begins drawing a new frame. This method will take control of Graphics RenderTarget.
    /// </summary>
    public void BeginFrame()
    {
        _gfx.SetRenderTarget(_target);
        Globals.SpriteBatch.Begin(transformMatrix: Translation);
    }
    
    /// <summary>
    /// Renders the most recently drawn frame to the graphics device and releases the Graphics RenderTarget.
    /// </summary>
    public void RenderFrame()
    {
        Globals.SpriteBatch.End();
        _gfx.SetRenderTarget(null);
        _gfx.Clear(Color.Black);
        Globals.SpriteBatch.Begin();
        Globals.SpriteBatch.Draw(_target, _drawDestination, Color.White);
        Globals.SpriteBatch.End();
    }

    public void ScaleViewport()
    {
        var screenSize = _gfx.PresentationParameters.Bounds;

        var scaleX = (float)screenSize.Width / _target.Width;
        var scaleY = (float)screenSize.Height / _target.Height;
        var scale = Math.Min(scaleX, scaleY);

        var newWidth = (int)Math.Truncate(_target.Width * scale);
        var newHeight = (int)Math.Truncate(_target.Height * scale);
        var left = (screenSize.Width - newWidth) / 2;
        var top = (screenSize.Height - newHeight) / 2;

        _drawDestination = new Rectangle(left, top, newWidth, newHeight);
        Console.Out.WriteLine($"Updated Viewport to {_drawDestination}");

        _gfx.Viewport = new Viewport(_drawDestination);
    }

    private void UpdateTranslationMatrix(Vector2 position)
    {
        var halfPlayerSize = 16;
        var dx = (_target.Width / 2f) - (position.X * Globals.TileSize + halfPlayerSize);
        var dy = (_target.Height / 2f) - (position.Y * Globals.TileSize  + halfPlayerSize);

        var newTranslation = Matrix.CreateTranslation(dx, dy, 0);
        Translation = newTranslation;
        Console.Out.WriteLine($"Updated Translation to \n{Translation}");
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