using Microsoft.Xna.Framework;

namespace gamespace.Model;

public abstract class PhysicsObj
{
    private bool _hasMovement;
    private bool _hasFriction;

    /// <summary>
    /// The world coordinate of this object's center.
    /// </summary>
    public Vector2 WorldCoordinate { get; protected set; }

    /// <summary>
    /// World width of this object.
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// World height of this object.
    /// </summary>
    public int Height { get; }

    /// <summary>
    /// True if this object can collide with others, false if objects should clip through it.
    /// </summary>
    public bool CanCollide { get; }


    protected PhysicsObj(Vector2 worldCoordinate, int width, int height, bool hasMovement, bool hasFriction,
        bool hasCollision)
    {
        WorldCoordinate = worldCoordinate;
        Width = width;
        Height = height;
        CanCollide = hasCollision;
        _hasMovement = hasMovement;
        _hasFriction = hasFriction;
    }

    public override string ToString()
    {
        return "World Coordinate: " + WorldCoordinate + " Width: " + Width + " Height: " + Height + " Has movement: " +
               _hasMovement + " Has friction: " + _hasFriction + " Has collision: " + CanCollide;
    }

    public abstract void FixedUpdate();
}