using System.Drawing;
using Rectangle = System.Drawing.Rectangle;

namespace gamespace.Model;

public class Room
{
    public Rectangle RoomBounds { get; init; }
    public bool IsConnectedToStart { get; set; }

    public Room(Rectangle room)
    {
        RoomBounds = room;
        IsConnectedToStart = false;
    }
}