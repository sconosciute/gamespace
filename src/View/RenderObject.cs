using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace gamespace.View;

public class RenderObject
{
    private Texture2D _spriteSheet;

    private int _framesPerRow;

    private int _framesPerCol;

    private Rectangle _source;

    public Rectangle Source
    {
        get => _source;
    }
    
    //https://community.monogame.net/t/spritebatch-draw-animation/16297/4
    public RenderObject(Texture2D spriteSheet, int framesPerRow, int framesPerCol)
    {
        _spriteSheet = spriteSheet;
        _framesPerRow = framesPerRow;
        _framesPerCol = framesPerCol;
        _source = new Rectangle(0, 0, 32, 32); //Sets initial to top right for now, will change on sprite sheets. 
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
    public void Draw(int x, int y)
    {
        //TODO: Implementation for drawing
    }
}