﻿using gamespace.View;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace gamespace;

public static class Globals
{
    /// <summary>
    /// The debug flag that allows the player to see further.
    /// </summary>
    public static bool DebugForceScale = false;
    
    /// <summary>
    /// Initializes globals.
    /// </summary>
    /// <param name="content">The content manager containing the textures.</param>
    /// <param name="spriteBatch">The spritebatch to draw.</param>
    /// <param name="font">The font for menus.</param>
    public static void Init(ContentManager content, SpriteBatch spriteBatch, SpriteFont font)
    {
        Content ??= content;
        SpriteBatch ??= spriteBatch;
        Font ??= font;
    }
    
    /// <summary>
    /// The screen space tile size in pixels.
    /// Use this value in render transformation matrices to convert from world to screen size.
    /// </summary>
    public const int TileSize = 16;

    /// <summary>
    /// The global content manager for the game.
    /// </summary>
    public static ContentManager Content { get; private set; }
    
    /// <summary>
    /// The global default SpriteBatch.
    /// </summary>
    public static SpriteBatch SpriteBatch { get; private set; }
    
    /// <summary>
    /// The font to use for all in-game text rendering
    /// </summary>
    public static SpriteFont Font { get; private set; }
    
    /// <summary>
    /// Changeable scale for textures.
    /// </summary>
    public static float Scale { get; private set; } = 1f;

    /// <summary>
    /// Updates the scale accordingly.
    /// </summary>
    public static void UpdateScale(GraphicsDevice gfx)
    {
        if (DebugForceScale)
        {
            Scale = 1f;
        }
        else
        {
            var windowScale = gfx.PresentationParameters.Bounds.Width / 640;
            Scale = windowScale;
        } 
        
    }

    /// <summary>
    /// The global LoggerFactory for getting loggers throughout the program.
    /// </summary>
    public static readonly ILoggerFactory LogFactory = LoggerFactory.Create(
        builder => builder
            .AddConsole()
            .AddDebug()
            .SetMinimumLevel(LogLevel.Debug));
}