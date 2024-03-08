using System.Collections.Generic;
using System.Drawing;
using Rectangle = System.Drawing.Rectangle;

namespace gamespace.Model;

public class Room
{
    public Rectangle RoomBounds { get; init; }
    public bool IsConnectedToStart { get; set; }
    public bool BeenVisited { get; set; }
    public List<Room> ConnectedRooms { get; set; }

    public Room(Rectangle room)
    {
        RoomBounds = room;
        IsConnectedToStart = false;
        ConnectedRooms = new List<Room>();
    }

    public override string ToString()
    {
        return "Room Bounds: " + RoomBounds;
    }
}