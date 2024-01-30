using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace gamespace.View;

public class RenderObject
{
    private Texture2D _spriteSheet;
    private Dictionary<string, Animation> _animations;

    public RenderObject(Texture2D spriteSheet, Dictionary<string, Animation> animations)
    {
        _spriteSheet = spriteSheet;
        _animations = animations;
    }

    public void Draw(int x, int y)
    {
        //TODO: Implementation for drawing
    }
}