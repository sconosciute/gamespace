using gamespace.Model;
using Microsoft.Xna.Framework;
using Rectangle = System.Drawing.Rectangle;

namespace gamespaceunittests;

public class RoomTest
{
    [Test]
    public void RoomConstructorTest()
    {
        var roomBounds = new Rectangle(0, 0, 3, 3);
        var room = new Room(roomBounds);
        string expectedData = "Room Bounds: " + roomBounds + " Is connected to start: " + "False";
        Assert.Multiple(() =>
        {
            Assert.That(room.ToString(), Is.EqualTo(expectedData));

            Assert.That(room.ConnectedRooms.TryGetValue(room, out var throwAway), Is.EqualTo(true));
        });
    }

    [Test]
    public void TestGetMiddleRight()
    {
        var roomBounds = new Rectangle(0, 0, 3, 3);
        var room = new Room(roomBounds);
        var midRight = room.GetMiddleRight();
        var expectedData = new Vector2(3, 1);
        Assert.That(midRight, Is.EqualTo(expectedData));
    }

    [Test]
    public void TestGetMiddleLeft()
    {
        var roomBounds = new Rectangle(0, 0, 3, 3);
        var room = new Room(roomBounds);
        var midRight = room.GetMiddleLeft();
        var expectedData = new Vector2(1, 1);
        Assert.That(midRight, Is.EqualTo(expectedData));
    }

    [Test]
    public void TestGetMiddleTop()
    {
        var roomBounds = new Rectangle(0, 0, 3, 3);
        var room = new Room(roomBounds);
        var midRight = room.GetMiddleTop();
        var expectedData = new Vector2(2, 0);
        Assert.That(midRight, Is.EqualTo(expectedData));
    }
    
    [Test]
    public void TestGetMiddleBottom()
    {
        var roomBounds = new Rectangle(0, 0, 3, 3);
        var room = new Room(roomBounds);
        var midRight = room.GetMiddleBottom();
        var expectedData = new Vector2(2, 2);
        Assert.That(midRight, Is.EqualTo(expectedData));
    }
}