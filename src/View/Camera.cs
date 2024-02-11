using System;
using gamespace.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace gamespace.View;

/// <summary>
/// The Camera handles translating world space to screen space for rendering.
/// </summary>
public class Camera
{
    //16:9 Widescreen resolution suitable for 16px tile sizes.
    private const int VResWidth = 640;
    private const int VResHeight = 360;

    private readonly RenderTarget2D _target;
    private readonly GraphicsDevice _gfx;
    private readonly Guid _playerId;

    /// <summary>
    /// The 2D translation Matrix from world to screen coordinates to be used with SpriteBatch for rendering.
    /// </summary>
    public Matrix Translation { get; private set; }

    /// <summary>
    /// Initiates a new fixed resolution camera that tracks the Player.
    /// </summary>
    /// <param name="playerId">Entity ID of player to follow.</param>
    /// <param name="graphicsDevice">The backend graphics device.</param>
    /// <param name="resolution">Point representing target resolution (width, height).</param>
    public Camera(Guid playerId, GraphicsDevice graphicsDevice, Point resolution)
    {
        _gfx = graphicsDevice;
        _target = new RenderTarget2D(_gfx, resolution.X, resolution.Y);
        _playerId = playerId;
        UpdateTranslationMatrix(Vector2.Zero);
    }

    public void HandleEntityEvent(Guid sender, EntityEventArgs args)
    {
        if (sender != _playerId) return;
        if (args.EventTopic != EntityEventType.Moved) return;
        UpdateTranslationMatrix(args.NewPosition);
    }
    
    

    private void UpdateTranslationMatrix(Vector2 position)
    {
        var dx = (Globals.WindowSize.X / 2) - (position.X * Globals.TileSize);
        var dy = (Globals.WindowSize.Y / 2) - (position.Y * Globals.TileSize);

        var newTranslation = Matrix.CreateTranslation(dx, dy, 0);
        Translation = newTranslation;
        Console.Out.WriteLine($"Updated Translation to \n{Translation}");
    }

    public void RenderFrame()
    {
        _gfx.SetRenderTarget(null);
        _gfx.Clear(Color.Black);
        
        
        Globals.SpriteBatch.Begin();
    }
}