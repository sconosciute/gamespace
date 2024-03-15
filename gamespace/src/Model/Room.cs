using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Rectangle = System.Drawing.Rectangle;

namespace gamespace.Model;

public class Room
{
    /// <summary>
    /// The bounds of the room object.
    /// </summary>
    public Rectangle RoomBounds { get; init; }
    
    /// <summary>
    /// Checks if the room is connected to the starting room.
    /// </summary>
    public bool IsConnectedToStart { get; set; }
    
    /// <summary>
    /// Checks if the room has been visited.
    /// </summary>
    public bool BeenVisited { get; set; }
    
    /// <summary>
    /// HashSet of rooms to keep track of all connected rooms.
    /// </summary>
    public HashSet<Room> ConnectedRooms { get; set; }

    /// <summary>
    /// Creates a room object.
    /// </summary>
    /// <param name="room">Rectangle representation of the space of the room.</param>
    public Room(Rectangle room)
    {
        RoomBounds = room;
        IsConnectedToStart = false;
        ConnectedRooms = new HashSet<Room> { this };
    }

    /// <summary>
    /// Gets the centered most right tile.
    /// </summary>
    public Vector2 GetMiddleRight()
    {
        return new Vector2(RoomBounds.Right, RoomBounds.Top + RoomBounds.Height / 2);
    }
    
    /// <summary>
    /// Gets the centered most left tile.
    /// </summary>
    public Vector2 GetMiddleLeft()
    {
        return new Vector2(RoomBounds.Left + 1, RoomBounds.Top + RoomBounds.Height / 2);
    }

    /// <summary>
    /// Gets the centered top tile.
    /// </summary>
    public Vector2 GetMiddleTop()
    {
        return new Vector2((RoomBounds.Left + 1) + RoomBounds.Width / 2, RoomBounds.Top);
    }
    
    /// <summary>
    /// Gets the centered bottom tile.
    /// </summary>
    public Vector2 GetMiddleBottom()
    {
        return new Vector2((RoomBounds.Left + 1) + RoomBounds.Width / 2, RoomBounds.Bottom - 1);
    }

    /// <summary>
    /// Checks if the room is equal to the object.
    /// </summary>
    /// <param name="obj">The object to compare.</param>
    public override bool Equals(object obj)
    {
        var newRoom = (Room)obj;
        return newRoom != null && RoomBounds.Location.Equals(newRoom.RoomBounds.Location);
    }

    /// <summary>
    /// ToString representation of the room bounds and if it's connected to the start.
    /// </summary>
    public override string ToString()
    {
        return "Room Bounds: " + RoomBounds + " Is connected to start: " + IsConnectedToStart;
    }
}