using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace gamespace;

public static class Globals
{
    
    /// <summary>
    /// The screen space tile size in pixels.
    /// Use this value in render transformation matrices to convert from world to screen size.
    /// </summary>
    public const int TileSize = 16;

    /// <summary>
    /// The global content manager for the game.
    /// </summary>
    public static ContentManager Content { get; set; }
    
    /// <summary>
    /// The global default SpriteBatch.
    /// </summary>
    public static SpriteBatch SpriteBatch { get; set; }
}