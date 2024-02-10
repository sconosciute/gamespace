using System;
using gamespace.Model;
using Microsoft.Xna.Framework;

namespace gamespace.View;

/// <summary>
/// The Camera handles translating world space to screen space for rendering.
/// </summary>
public class Camera
{
    private readonly Guid _playerId;

    /// <summary>
    /// The 2D translation Matrix from world to screen coordinates to be used with SpriteBatch for rendering.
    /// </summary>
    public Matrix Translation { get; private set; }

    public Camera(Guid playerId)
    {
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
}