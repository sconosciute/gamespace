using System;
using gamespace.View;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace gamespace.Model;

public abstract class Entity : PhysicsObj
{
    private const float BaseMoveSpeed = 0.1f;

    private RenderObject _sprite;
    private float _xSpeed;
    private float _ySpeed;
    private World _world;

    protected Entity(int moveSpeed, RenderObject sprite, int x, int y, int width, int height, bool hasCollision,
        World world) : base(x, y, width, height, hasCollision, true, true, BaseMoveSpeed)
    {
        _sprite = sprite;
        _world = world;
    }

    public void FixedUpdate()
    {
        var newPos = new Vector2(X + _xSpeed, Y + _ySpeed);

        var bbx1 = (int)Math.Min(newPos.X, X);
        var bbx2 = (int)Math.Ceiling(Math.Max(newPos.X, X));
        var bby1 = (int)Math.Min(newPos.Y, Y);
        var bby2 = (int)Math.Ceiling(Math.Max(newPos.Y, Y));

        Tile checkTile;
        for (int worldX = bbx1; worldX <= bbx2; worldX++)
        {
            if (!_world.IsInBounds(worldX, 0)) continue;
            for (int worldY = bby1; worldY <= bby2; worldY++)
            {
                if (!_world.IsInBounds(0, worldY)) continue;
                checkTile = _world[worldX, worldY];
                if (checkTile.CanCollide)
                {
                    CheckCollision(checkTile.Prop);
                }
            }
        }
    }

    private void CheckCollision(PhysicsObj other)
    {
        //Speculative collision using Minkowski difference
        //Reduce other to a point, expand this aabb by dims of other aabb, check for intersection!
        var othCenter = new Vector2(other.X, other.Y);

        var bbWidth = Width + other.Width;
        var bbHeight = Height + other.Height;
        var colVector = new Vector2(othCenter.X - X, othCenter.Y - Y);

        if (Math.Abs(colVector.X) > Math.Abs(colVector.Y))
        {
            _xSpeed = _xSpeed > (colVector.X - bbWidth / 2f) ? colVector.X : _xSpeed;
        }
        else
        {
            _ySpeed = _ySpeed > (colVector.Y - bbHeight / 2f) ? colVector.Y : _ySpeed;
        }
    }
    public RenderObject Sprite { get; set; }
}