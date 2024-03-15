﻿using Microsoft.Xna.Framework;

namespace gamespace.Model;

public class Prop : PhysicsObj
{
    public Prop(Vector2 worldCoordinate, float width, float height, bool hasCollision) 
        : base(worldCoordinate, width, height, false, false, hasCollision)
    {
    }

    public override void FixedUpdate()
    {
        //TODO: Does Prop ever need to actually update?
    }
}