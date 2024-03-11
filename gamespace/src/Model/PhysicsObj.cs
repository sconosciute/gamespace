using System;
using Microsoft.Xna.Framework;

namespace gamespace.Model;

public abstract class PhysicsObj : IDisposable
{
    private bool _hasMovement;
    private bool _hasFriction;

    /// <summary>
    /// The world coordinate of this object's center.
    /// </summary>
    public Vector2 WorldCoordinate
    {
        get;
        protected set;
    }

    /// <summary>
    /// World width of this object.
    /// </summary>
    public float Width { get; }

    /// <summary>
    /// World height of this object.
    /// </summary>
    public float Height { get; }

    /// <summary>
    /// True if this object can collide with others, false if objects should clip through it.
    /// </summary>
    public bool CanCollide { get; }


    protected PhysicsObj(Vector2 worldCoordinate, float width, float height, bool hasMovement, bool hasFriction, bool hasCollision)
    {
        WorldCoordinate = worldCoordinate;
        Width = width;
        Height = height;
        CanCollide = hasCollision;
        _hasMovement = hasMovement;
        _hasFriction = hasFriction;
    }

    public abstract void FixedUpdate();

    public void Dispose()
    {
        // TODO release managed resources here
    }
}