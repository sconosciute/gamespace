using System;
using gamespace.Util;
using Microsoft.Extensions.Logging;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace gamespace.Model.Entities
{
    public abstract class Entity : PhysicsObj
    {
        /// <summary>
        /// Default speed for all entities.
        /// </summary>
        private const float DefaultEntSpeed = 0.15f;

        /// <summary>
        /// The world that the entities will be rendered in.
        /// </summary>
        protected readonly World World;
        
        /// <summary>
        /// Base move speed for entities.
        /// </summary>
        private float _baseMoveSpeed = DefaultEntSpeed;
        
        /// <summary>
        /// The move speed of the entity.
        /// </summary>
        private Vector2 _moveSpeed;
        
        /// <summary>
        /// Debug logger.
        /// </summary>
        private ILogger _log;
        
        /// <summary>
        /// Gets or sets the last moving direction of the entity.
        /// </summary>
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

        /// <summary>
        /// Gets of initializes the ID for the entity.
        /// </summary>
        public Guid EntityId { get; init; }

        /// <summary>
        /// Gets and sets the move speed of the entity.
        /// </summary>
        protected Vector2 MoveSpeed
        {
            get => _moveSpeed;
            set => _moveSpeed = value;
        }

        /// <summary>
        /// Event handler for entity events.
        /// </summary>
        public event EventHelper.EntityEventHandler EntityEvent;

        /// <summary>
        /// Invokes the entity event.
        /// </summary>
        /// <param name="args">The current entity event.</param>
        private void OnEntityEvent(EventHelper.EntityEventArgs args)
        {
            EntityEvent?.Invoke(EntityId, args);
        }
        

        /// <summary>
        /// Creates an entity object to render onto the world.
        /// </summary>
        /// <param name="width">Width of the entity.</param>
        /// <param name="height">Height of the entity.</param>
        /// <param name="world">The world the entity resides in.</param>
        /// <param name="worldCoordinate">The world coordinate where the entity will spawn.</param>
        protected Entity(float width, float height, World world, Vector2 worldCoordinate)
            : base(worldCoordinate, width, height, true, true, true)
        {
            _log = Globals.LogFactory.CreateLogger<Entity>();
            World = world;
            MoveSpeed = Vector2.Zero;
            EntityId = Guid.NewGuid();
        }

        /// <summary>
        /// Updates the position of the entity.
        /// </summary>
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
            var args = new EventHelper.EntityEventArgs()
            {
                EventTopic = EventHelper.EntityEventType.Moved,
                NewPosition = WorldCoordinate,
                OldPosition = oldPos
            };
            OnEntityEvent(args);
        }
        
        /// <summary>
        /// Sends entity object to render object so it can be rendered.
        /// </summary>
        public event EventHelper.SendEntityToUnrender SendObjToRenderObj;
        
        /// <summary>
        /// Sends the entity object to the world so the entity can reside there.
        /// </summary>
        public event EventHelper.SendEntityToUnrender SendObjToWorldBuilder;

        /// <summary>
        /// Unregisters the entity if it dies.
        /// </summary>
        protected virtual void OnDeath() 
        {
            SendObjToRenderObj?.Invoke(EntityId);
            SendObjToWorldBuilder?.Invoke(EntityId);
        }

        /// <summary>
        /// Translates the entity's direction.
        /// </summary>
        /// <param name="translation">The translation of the entity direction.</param>
        /// <param name="curPos">The current position of the entity.</param>
        protected virtual void Translate(Vector2 translation, ref Vector2 curPos)
        {
            var newPos = new Vector2(curPos.X + translation.X, curPos.Y + translation.Y);
            var bbx1 = (int)Math.Floor(Math.Min(newPos.X, curPos.X));
            var bbx2 = (int)Math.Ceiling(Math.Max(newPos.X, curPos.X));
            var bby1 = (int)Math.Floor(Math.Min(newPos.Y, curPos.Y));
            var bby2 = (int)Math.Ceiling(Math.Max(newPos.Y, curPos.Y));
        
            for (var worldX = bbx1; worldX <= bbx2; worldX++)
            {
                if (!World.IsInBounds(worldX, 0)) continue;
                for (var worldY = bby1; worldY <= bby2; worldY++)
                {
                    if (!World.IsInBounds(0, worldY)) continue;
                    var checkTile = World[worldX, worldY];
                    if (checkTile is { CanCollide: true })
                    {
                        CheckCollision(checkTile.Prop, ref translation, curPos);
                    }
                }
            }

            curPos.X += translation.X;
            curPos.Y += translation.Y;
        }

        /// <summary>
        /// Checks if the entity has collided with something.
        /// </summary>
        /// <param name="other">Other object that the collision occurred with.</param>
        /// <param name="translation">The translation to adjust collision.</param>
        /// <param name="curPos">The current position of the collision.</param>
        protected bool CheckCollision(in PhysicsObj other, ref Vector2 translation, in Vector2 curPos)
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

        /// <summary>
        /// Adjusts the collision accordingly.
        /// </summary>
        /// <param name="collisionMagnitude">The collision magnitude.</param>
        /// <param name="moveMagnitude">The movement magnitude.</param>
        /// <param name="boundAdjust">The bounds to adjust to.</param>
        private static float AdjustCollision(float collisionMagnitude, float moveMagnitude, float boundAdjust)
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