using System;
using gamespace.Util;
using Loyc.Geometry;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace gamespace.Model;

public class Projectile : Entity 
{
    private int BulletDamage = 10;
    private Guid _sender;
    public Projectile(float width, float height, World world, Vector2 worldCoordinate, Vector2 direction, Guid sender) : base(width, height, world, worldCoordinate) //How the hell to move proj idk
    {
        if (direction.Equals(Vector2.Zero))
        {
            direction = new Vector2(0.12f, 0.12f);
        }
        //var positionToSpawn = Vector2.Add(worldCoordinate, Vector2.Multiply(direction, new Vector2(12, 12)) );

        //WorldCoordinate = positionToSpawn;

        MoveSpeed = direction; //* 0.1f; //* 0.1f; //* 0.1f;
        _sender = sender;
    }
    //TODO: Change this up to entity level, idk how inheritance works with events I'm lost, please send help.
    /*public event EventHelper.SendEntityToUnrender SendObjToRenderObj;
    public event EventHelper.SendEntityToUnrender SendObjToWorldBuilder;

    private void OnDeath() 
    {
        SendObjToRenderObj?.Invoke(EntityId);
        SendObjToWorldBuilder?.Invoke(EntityId);
    }*/
    
    
    protected override void Translate(Vector2 translation, ref Vector2 curPos)
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
                    if (CheckCollision(checkTile.Prop, ref translation, curPos))
                    {
                        //Need to kill object and unrender it
                        OnDeath(); //If you replace onDeath with this they bounce!!! MoveSpeed = MoveSpeed * -1;
                        
                        //BulletDamage = 0;
                    }
                }   //Adding in logic to see if the bullet hits any players
                else
                {
                    //itterate through world entity list
                    foreach (var ent in _world.Entites)
                    {
                        //if (ent.WorldCoordinate.Equals(new Vector2(worldX, worldY)))
                        Vector2 posToCheck = new Vector2((int)ent.WorldCoordinate.X, (int)ent.WorldCoordinate.Y);
                        if (posToCheck.Equals(new Vector2(worldX, worldY)) && !ent.EntityId.Equals(_sender))
                        {
                            ent.AddHealth(-BulletDamage);
                            OnDeath();
                            
                            //GC.Collect();
                            //BulletDamage = 0;
                        }
                    }
                }
            }
        }

        curPos.X += translation.X;
        curPos.Y += translation.Y;
    }
    
    
}