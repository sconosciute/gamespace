using System;
using gamespace.View;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace gamespace.Model;

public abstract class Entity : PhysicsObj
{
    private const float DefaultEntSpeed = 0.1f;

    private World _world;
    private float _baseMoveSpeed = DefaultEntSpeed;
    private Vector2 _moveSpeed;

    /// <summary>
    /// The 0-2 fraction of a tile this entity can move in one update.
    /// Clamps between 0-2 without raising exception if out of bounds.
    /// </summary>
    public float BaseMoveSpeed
    {
        get => _baseMoveSpeed;
        protected set => _baseMoveSpeed = Math.Clamp(value, 0f, 2f);
    }

    protected Vector2 MoveSpeed
    {
        get => _moveSpeed;
        set => _moveSpeed = value;
    }

    protected Entity(int width, int height, World world, Vector2 worldCoordinate) : base(worldCoordinate, width, height, true, true)
    {
        _world = world;
        MoveSpeed = Vector2.Zero;
    }

    public override void FixedUpdate()
    {
        var x = WorldCoordinate.X;
        var y = WorldCoordinate.Y;
        var newPos = new Vector2(x + MoveSpeed.X, y + MoveSpeed.Y);

        var bbx1 = (int)Math.Min(newPos.X, x);
        var bbx2 = (int)Math.Ceiling(Math.Max(newPos.X, x));
        var bby1 = (int)Math.Min(newPos.Y, y);
        var bby2 = (int)Math.Ceiling(Math.Max(newPos.Y, y));

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

        WorldCoordinate = new Vector2(x + _moveSpeed.X, y + _moveSpeed.Y);

    }

    private void CheckCollision(PhysicsObj other)
    {
        //Speculative collision using Minkowski difference
        //Reduce other to a point, expand this aabb by dims of other aabb, check for intersection!
        var othCenter = other.WorldCoordinate;

        var bbWidth = Width + other.Width;
        var bbHeight = Height + other.Height;
        var colVector = new Vector2(othCenter.X - WorldCoordinate.X, othCenter.Y - WorldCoordinate.Y);

        _moveSpeed = MoveSpeed;
        if (Math.Abs(colVector.X) > Math.Abs(colVector.Y))
        {
            _moveSpeed.X = _moveSpeed.X > (colVector.X - bbWidth / 2f) ? colVector.X : _moveSpeed.X;
        }
        else
        {
            _moveSpeed.Y = _moveSpeed.Y > (colVector.Y - bbHeight / 2f) ? colVector.Y : _moveSpeed.Y;
        }
    }
}