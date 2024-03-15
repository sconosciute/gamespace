using System;
using System.Linq;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace gamespace.Model.Entities;

public class Projectile : Entity 
{
    /// <summary>
    /// The damage a bullet does to an entity.
    /// </summary>
    private const int BulletDamage = 10;
    
    /// <summary>
    /// ID to keep track of who shot the bullet.
    /// </summary>
    private readonly Guid _sender;
    
    /// <summary>
    /// Creates a projectile object.
    /// </summary>
    /// <param name="width">Width of the projectile.</param>
    /// <param name="height">Height of the projectile.</param>
    /// <param name="world">The world the projectile is in.</param>
    /// <param name="worldCoordinate">The coordinate of the projectile.</param>
    /// <param name="direction">The direction the projectile is going.</param>
    /// <param name="sender">The entity that shot the projectile.</param>
    public Projectile(float width, float height, World world, Vector2 worldCoordinate, Vector2 direction, Guid sender) : base(width, height, world, worldCoordinate) //How the hell to move proj idk
    {
        if (direction.Equals(Vector2.Zero))
        {
            direction = new Vector2(0.12f, 0.12f);
        }

        MoveSpeed = direction * 1.5f; 
        _sender = sender;
    }
    
    /// <summary>
    /// Translates the position of the projectile.
    /// </summary>
    /// <param name="translation">Movement translation.</param>
    /// <param name="curPos">The current position.</param>
    protected override void Translate(Vector2 translation, ref Vector2 curPos)
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
                    if (CheckCollision(checkTile.Prop, ref translation, curPos))
                    {
                        OnDeath(); //If you replace onDeath with this they bounce!!! MoveSpeed = MoveSpeed * -1;
                    }
                }
                else
                {
                    var x = worldX;
                    var y = worldY;
                    foreach (var ent in from ent in World.Entities let posToCheck = new Vector2((int)ent.WorldCoordinate.X, 
                                 (int)ent.WorldCoordinate.Y) where posToCheck.Equals(new Vector2(x, y)) && !ent.EntityId.Equals(_sender) select ent)
                    {
                        ent.AddHealth(-BulletDamage);
                        OnDeath();
                    }
                }
            }
        }

        curPos.X += translation.X;
        curPos.Y += translation.Y;
    }
    
    
}