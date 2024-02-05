using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace gamespace.View;

public class RenderObject
{
    private static readonly Color DEFAULT_COLOR = Color.White;
    private static readonly Vector2 DEFAULT_SCALE = Vector2.One;
    
    private Texture2D _spriteSheet;

    private int _framesPerRow;
    
    private int _framesPerCol;

    private Rectangle _source;

    private Vector2 _position;
    

    public Rectangle Source
    {
        get => _source;
    }
    public Vector2 Position
    {
        get => _position;
        set
        {
            _position = value;
        }
        
    }
    
    //https://community.monogame.net/t/spritebatch-draw-animation/16297/4
    public RenderObject(Texture2D spriteSheet, Vector2 position, int framesPerRow, int framesPerCol)
    {
        _spriteSheet = spriteSheet;
        _framesPerRow = framesPerRow;
        _framesPerCol = framesPerCol;
        _position = position;
        _source = new Rectangle((int)position.X, (int)position.Y, Width(), Height()); //Sets initial to top right for now, will change on sprite sheets. 
    }
    public int Column(int currentFrame)
    {
        return currentFrame % _framesPerRow;
    }
    public int Row(int currentFrame)
    {
        return currentFrame / _framesPerRow;
    }
    public int Width()
    { return _spriteSheet.Width / _framesPerCol; }
    public int Height()
    { return _spriteSheet.Height / _framesPerRow; }
    public Rectangle SourceRectangle(int currentFrame)
    {
        int frameWidth = Width();
        int frameHeight = Height();
        int column = Column(currentFrame) * frameWidth;
        int row = Row(currentFrame) * frameHeight;
        _source = new Rectangle(column, row, frameWidth, frameHeight);
        return _source;
    }
    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_spriteSheet, _position, _source, DEFAULT_COLOR, 0f, Vector2.Zero, DEFAULT_SCALE, SpriteEffects.None, 1f);
        //TODO: Implementation for drawing
    }
}