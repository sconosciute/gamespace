using System;
using Microsoft.Xna.Framework;

namespace gamespace.Model;

public abstract class PhysicsObj
{
    private bool _hasMovement;
    private bool _hasFriction;
    private float _moveSpeed;

    /// <summary>
    /// The world X coordinate of this object's center.
    /// </summary>
    public float X { get; protected set; }

    /// <summary>
    /// The world Y coordinate of this object's center.
    /// </summary>
    public float Y { get; protected set; }

    /// <summary>
    /// World width of this object.
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// World height of this object.
    /// </summary>
    public int Height { get; }

    /// <summary>
    /// The 0-2 fraction of a tile this entity can move in one update.
    /// Clamps between 0-2 without raising exception if out of bounds.
    /// </summary>
    public float MoveSpeed
    {
        get => _moveSpeed;
        protected set => _moveSpeed = Math.Clamp(value, 0f, 2f);
    }

    /// <summary>
    /// True if this object can collide with others, false if objects should clip through it.
    /// </summary>
    public bool CanCollide { get; }


    protected PhysicsObj(int x, int y, int width, int height, bool hasCollision, bool hasMovement, bool hasFriction,
        float moveSpeed)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
        CanCollide = hasCollision;
        _hasMovement = hasMovement;
        _hasFriction = hasFriction;
        MoveSpeed = moveSpeed;
    }

    public abstract void Update(GameTime gameTime);
}