using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.Xna.Framework;
using Rectangle = System.Drawing.Rectangle;

namespace gamespace.Model;

public class Room
{
    public Rectangle RoomBounds { get; init; }
    public bool IsConnectedToStart { get; set; }
    public bool BeenVisited { get; set; }
    public HashSet<Room> ConnectedRooms { get; set; }

    public Room(Rectangle room)
    {
        RoomBounds = room;
        IsConnectedToStart = false;
        ConnectedRooms = new HashSet<Room>();
        ConnectedRooms.Add(this);
    }

    public Vector2 GetMiddleRight()
    {
        /*if (Math.Abs(RoomBounds.X) > RoomBounds.Width)
        {
            return new Vector2(RoomBounds.Right, RoomBounds.Bottom + RoomBounds.Top);
        }*/

        return new Vector2(RoomBounds.Right, RoomBounds.Top + RoomBounds.Height / 2);
    }
    
    public Vector2 GetMiddleLeft()
    {
        //return new Vector2(RoomBounds.X, RoomBounds.Y);
        //return new Vector2(RoomBounds.Left, RoomBounds.Top);
        /*if (Math.Abs(RoomBounds.X) > RoomBounds.Width)
        {
            return new Vector2(RoomBounds.Left + 1, RoomBounds.Bottom + RoomBounds.Top);
        }*/

        return new Vector2(RoomBounds.Left + 1, RoomBounds.Top + RoomBounds.Height / 2);
    }

    public Vector2 GetMiddleTop()
    {
        /*if (Math.Abs(RoomBounds.Y) > RoomBounds.Height)
        {
            return new Vector2((RoomBounds.Right + RoomBounds.Left + 1), RoomBounds.Top);
        }*/
        
        return new Vector2((RoomBounds.Left + 1) + RoomBounds.Width / 2, RoomBounds.Top);
    }
    
    public Vector2 GetMiddleBottom()
    {
        /*if (Math.Abs(RoomBounds.Y) > RoomBounds.Height)
        {
            return new Vector2((RoomBounds.Right + RoomBounds.Left + 1), RoomBounds.Bottom - 1);
        }*/
        
        return new Vector2((RoomBounds.Left + 1) + RoomBounds.Width / 2, RoomBounds.Bottom - 1);
    }

    public override bool Equals(object obj)
    {
        var newRoom = (Room)obj;
        return RoomBounds.Location.Equals(newRoom.RoomBounds.Location);
    }

    public override string ToString()
    {
        return "Room Bounds: " + RoomBounds + " Is connected to start: " + IsConnectedToStart;
    }
}