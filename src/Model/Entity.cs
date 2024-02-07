using System;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace gamespace.Model;

public abstract class Entity : PhysicsObj
{
    private const float DefaultEntSpeed = 0.1f;

    private readonly World _world;
    private float _baseMoveSpeed = DefaultEntSpeed;
    private Vector2 _moveSpeed;

    public delegate void EntityEventHandler(Entity sender, EntityEventArgs args);

    public event EntityEventHandler EntityEvent;

    protected virtual void RaiseEntityEvent(EntityEventArgs args)
    {
        EntityEvent?.Invoke(this, args);
    }

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
        var oldPos = WorldCoordinate;
        var newPos = new Vector2(oldPos.X + MoveSpeed.X, oldPos.Y + MoveSpeed.Y);

        var bbx1 = (int)Math.Min(newPos.X, oldPos.X);
        var bbx2 = (int)Math.Ceiling(Math.Max(newPos.X, oldPos.X));
        var bby1 = (int)Math.Min(newPos.Y, oldPos.Y);
        var bby2 = (int)Math.Ceiling(Math.Max(newPos.Y, oldPos.Y));

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

        WorldCoordinate = new Vector2(oldPos.X + _moveSpeed.X, oldPos.Y + _moveSpeed.Y);
        
        if (oldPos == newPos) return;
        var args = new EntityEventArgs()
        {
            Type = EntityEventType.Moved,
            NewPosition = WorldCoordinate,
            OldPosition = oldPos
        };
        RaiseEntityEvent(args);

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

public class EntityEventArgs
{
    /// <summary>
    /// The type/topic of this event.
    /// </summary>
    public EntityEventType Type { get; init; }
    
    /// <summary>
    /// The previous position of this Entity.
    /// </summary>
    public Vector2 OldPosition { get; init; }
    
    /// <summary>
    /// The new position of this Entity.
    /// </summary>
    public Vector2 NewPosition { get; init; }
}

public enum EntityEventType
{
    Moved
}