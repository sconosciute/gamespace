using System;
using Microsoft.Xna.Framework;

namespace gamespace.Model;

public abstract class PhysicsObj : IDisposable
{
    /// <summary>
    /// Determines if the physics object has movement.
    /// </summary>
    private bool _hasMovement;
    
    /// <summary>
    /// Determines if the physics object has friction.
    /// </summary>
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

    /// <summary>
    /// Creates a physics object.
    /// </summary>
    /// <param name="worldCoordinate">The coordinate of the object.</param>
    /// <param name="width">The width of the object.</param>
    /// <param name="height">The height of the object.</param>
    /// <param name="hasMovement">Determines if the object has movement.</param>
    /// <param name="hasFriction">Determines if the object has friction.</param>
    /// <param name="hasCollision">Determines if the object has collision.</param>
    protected PhysicsObj(Vector2 worldCoordinate, float width, float height, bool hasMovement, bool hasFriction, bool hasCollision)
    {
        WorldCoordinate = worldCoordinate;
        Width = width;
        Height = height;
        CanCollide = hasCollision;
        _hasMovement = hasMovement;
        _hasFriction = hasFriction;
    }

    /// <summary>
    /// Fixed update for the object.
    /// </summary>
    public abstract void FixedUpdate();

    /// <summary>
    /// Disposes of the object.
    /// </summary>
    public void Dispose()
    {
        // TODO release managed resources here
    }
}