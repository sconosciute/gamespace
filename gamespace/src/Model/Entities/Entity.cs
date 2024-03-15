﻿using System;
using gamespace.Util;
using Microsoft.Extensions.Logging;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace gamespace.Model
{
    public abstract class Entity : PhysicsObj
    {
        private const float DefaultEntSpeed = 0.15f;

        protected readonly World _world;
        private float _baseMoveSpeed = DefaultEntSpeed;
        private Vector2 _moveSpeed;
        private ILogger _log;
        public Vector2 LastMovingDirection { get; protected set; }
        
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

        public Vector2 MoveSpeed //prot to pub
        {
            get => _moveSpeed;
            set => _moveSpeed = value;
        }

        public event EventHelper.EntityEventHandler EntityEvent;

        protected virtual void OnEntityEvent(EventHelper.EntityEventArgs args)
        {
            EntityEvent?.Invoke(EntityId, args);
        }
        

        protected Entity(float width, float height, World world, Vector2 worldCoordinate)
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
            /*if (oldPos != Vector2.Zero)
            {
                
            }*/
            var newPos = WorldCoordinate;

            var xTranslation = new Vector2(_moveSpeed.X, 0f);
            var yTranslation = new Vector2(0f, _moveSpeed.Y);
            Translate(xTranslation, ref newPos);
            Translate(yTranslation, ref newPos);

            WorldCoordinate = newPos;
            /*if (newPos != Vector2.Zero)
            {
                LastMovingDirection = newPos;
            }*/

            if (oldPos == newPos) return;
            var args = new EventHelper.EntityEventArgs()
            {
                EventTopic = EventHelper.EntityEventType.Moved,
                NewPosition = WorldCoordinate,
                OldPosition = oldPos
            };
            OnEntityEvent(args);
        }
        
        public event EventHelper.SendEntityToUnrender SendObjToRenderObj;
        public event EventHelper.SendEntityToUnrender SendObjToWorldBuilder;

        protected virtual void OnDeath() 
        {
            SendObjToRenderObj?.Invoke(EntityId);
            SendObjToWorldBuilder?.Invoke(EntityId);
        }

        protected virtual void Translate(Vector2 translation, ref Vector2 curPos)     //Making this protected and virtual wafor projectiles to work
        {
            var newPos = new Vector2(curPos.X + translation.X, curPos.Y + translation.Y);
            var bbx1 = (int)Math.Floor(Math.Min(newPos.X, curPos.X));
            var bbx2 = (int)Math.Ceiling(Math.Max(newPos.X, curPos.X));
            var bby1 = (int)Math.Floor(Math.Min(newPos.Y, curPos.Y));
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

        protected bool CheckCollision(in PhysicsObj other, ref Vector2 translation, in Vector2 curPos) //Changed it so it returns if theres a colision.
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
                return true;
            }
            else
            {
                return false;
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

    
}