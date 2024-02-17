using System;
using Microsoft.Extensions.Logging;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace gamespace.Model;

public abstract class Entity : PhysicsObj
{
    private const float DefaultEntSpeed = 0.1f;

    private readonly World _world;
    private float _baseMoveSpeed = DefaultEntSpeed;
    private Vector2 _moveSpeed;
    private ILogger _log;

    public delegate void EntityEventHandler(Guid sender, EntityEventArgs args);

    public event EntityEventHandler EntityEvent;

    protected virtual void OnEntityEvent(EntityEventArgs args)
    {
        EntityEvent?.Invoke(EntityId, args);
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

    public Guid EntityId { get; init; }

    protected Vector2 MoveSpeed
    {
        get => _moveSpeed;
        set => _moveSpeed = value;
    }

    protected Entity(int width, int height, World world, Vector2 worldCoordinate)
        : base(worldCoordinate, width, height, true, true, true)
    {
        _log = Globals.LogFactory.CreateLogger<Entity>();
        _world = world;
        MoveSpeed = Vector2.Zero;
        EntityId = Guid.NewGuid();
    }

    public override void FixedUpdate()
    {
        var oldPos = WorldCoordinate;
        var newPos = WorldCoordinate;

        var xTranslation = new Vector2(_moveSpeed.X, 0f);
        var yTranslation = new Vector2(0f, _moveSpeed.Y);
        Translate(xTranslation, ref newPos);
        Translate(yTranslation, ref newPos);

        WorldCoordinate = newPos;

        if (oldPos == newPos) return;
        var args = new EntityEventArgs()
        {
            EventTopic = EntityEventType.Moved,
            NewPosition = WorldCoordinate,
            OldPosition = oldPos
        };
        OnEntityEvent(args);
    }

    private void Translate(Vector2 translation, ref Vector2 curPos)
    {
        var newPos = new Vector2(curPos.X + translation.X, curPos.Y + translation.Y);
        var bbx1 = (int)Math.Min(newPos.X, curPos.X);
        var bbx2 = (int)Math.Ceiling(Math.Max(newPos.X, curPos.X));
        var bby1 = (int)Math.Min(newPos.Y, curPos.Y);
        var bby2 = (int)Math.Ceiling(Math.Max(newPos.Y, curPos.Y));
        
        for (var worldX = bbx1; worldX <= bbx2; worldX++)
        {
            if (!_world.IsInBounds(worldX, 0)) continue;
            for (var worldY = bby1; worldY <= bby2; worldY++)
            {
                if (!_world.IsInBounds(0, worldY)) continue;
                var checkTile = _world[worldX, worldY];
                if (checkTile is { CanCollide: true })
                {
                    CheckCollision(checkTile.Prop, ref translation, curPos);
                }
            }
        }

        curPos.X += translation.X;
        curPos.Y += translation.Y;
    }

    private void CheckCollision(in PhysicsObj other, ref Vector2 translation, in Vector2 curPos)
    {
        var othCenter = other.WorldCoordinate;
        var colVector = new Vector2(othCenter.X - curPos.X, othCenter.Y - curPos.Y);
        var oldMove = translation;

        if (Math.Abs(colVector.X) > Math.Abs(colVector.Y))
        {
            translation.X = AdjustCollision(colVector.X, translation.X, other.Width);
        }
        else
        {
            translation.Y = AdjustCollision(colVector.Y, translation.Y, other.Height);
        }

        if (translation != oldMove)
        {
            _log.LogDebug("{id} at {myPos} Collision detected at {pos} : Old move vector was {old}, move vector is now {new}.",
                EntityId,
                WorldCoordinate,
                other.WorldCoordinate,
                oldMove,
                translation);
        }
    }

    private float AdjustCollision(float collisionMagnitude, float moveMagnitude, float boundAdjust)
    {
        //If the collision is happening behind us then we aren't colliding
        if (!(collisionMagnitude > 0) == (moveMagnitude > 0)) return moveMagnitude;
        if (collisionMagnitude < 0) boundAdjust *= -1;
        
        var absCol = Math.Abs(collisionMagnitude);
        var absMove = Math.Abs(moveMagnitude);
        
        return absMove > (absCol - Math.Abs(boundAdjust)) ? collisionMagnitude - (boundAdjust) : moveMagnitude;

    }
}

public class EntityEventArgs
{
    /// <summary>
    /// The type/topic of this event.
    /// </summary>
    public EntityEventType EventTopic { get; init; }

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