using gamespace.Model;
using gamespace.Model.Props;
using Microsoft.Xna.Framework;
using Rectangle = System.Drawing.Rectangle;

namespace gamespaceunittests;

public class WorldTest
{
    [Test]
    public void WorldConstructorTest()
    {
        var testerWorld = new World(3, 3);
        var expectedData = "Min X: " + "-1" + " Min Y: " + "-1" + " Max X: " + "1" + " Max Y: " + "1";
        Assert.That(testerWorld.ToString(), Is.EqualTo(expectedData));
    }

    [Test]
    public void WorldConstructorZeroWidthTest()
    {
        Assert.Throws(typeof(ArithmeticException), WorldConstructorZeroWidthTestHelper);
    }

    private void WorldConstructorZeroWidthTestHelper()
    {
        var zeroWidthWorld = new World(0, 1);
    }
    
    [Test]
    public void WorldConstructorNegativeWidthTest()
    {
        Assert.Throws(typeof(ArithmeticException), WorldConstructorNegativeWidthTestHelper);
    }

    private void WorldConstructorNegativeWidthTestHelper()
    {
        var negativeWidthWorld = new World(-1, 1);
    }
    
    [Test]
    public void WorldConstructorZeroHeightTest()
    {
        Assert.Throws(typeof(ArithmeticException), WorldConstructorZeroHeightTestHelper);
    }

    private void WorldConstructorZeroHeightTestHelper()
    {
        var zeroHeightWorld = new World(1, 0);
    }
    
    [Test]
    public void WorldConstructorNegativeHeightTest()
    {
        Assert.Throws(typeof(ArithmeticException), WorldConstructorNegativeHeightTestHelper);
    }

    private void WorldConstructorNegativeHeightTestHelper()
    {
        var negativeHeightWorld = new World(1, -1);
    }

    [Test]
    public void WorldCheckOverLapTrue()
    {
        var world = new World(31, 31);
        var Room1Bounds = new Rectangle(1, 1, 8, 8);
        var Room1 = new Room(Room1Bounds);
        world.Rooms.Add(Room1);
        var Room2Bounds = new Rectangle(0, 0, 8, 8);
        var room2 = new Room(Room2Bounds);

        Assert.That(world.CheckRoomOverlap(room2), Is.EqualTo(true));
    }
    
    [Test]
    public void WorldCheckOverLapFalse()
    {
        var world = new World(31, 31);
        var Room1Bounds = new Rectangle(10, 10, 8, 8);
        var Room1 = new Room(Room1Bounds);
        world.Rooms.Add(Room1);
        var Room2Bounds = new Rectangle(0, 0, 8, 8);
        var room2 = new Room(Room2Bounds);

        Assert.That(world.CheckRoomOverlap(room2), Is.EqualTo(false));
    }

    [Test]
    public void WorldTestIsInBoundsFalse()
    {
        var world = new World(9, 9);
        Assert.That(world.IsInBounds(10, 10), Is.EqualTo(false));
    }
    [Test]
    public void WorldTestOutOfBoundsTrue()
    {
        var world = new World(9, 9);
        Assert.That(world.IsInBounds(0, 0), Is.EqualTo(true));
    }

    [Test]
    public void WorldTestGetIsFloorTrue()
    {
        var world = new World(9, 9);
        Prop prop = new Prop(Vector2.Zero, 1, 1, false);
        var tile = new Tile(prop);
        world.ForcePlaceFloor(Vector2.Zero, tile);
        Assert.That(world.GetIsFloor(Vector2.Zero), Is.EqualTo(true));
    }
    [Test]
    public void WorldTestGetIsFloorFalse()
    {
        var world = new World(9, 9);
        Prop prop = new Prop(Vector2.Zero, 1, 1, true);
        var tile = new Tile(prop);
        world.ForcePlaceFloor(Vector2.Zero, tile);
        Assert.That(world.GetIsFloor(Vector2.Zero), Is.EqualTo(false));
    }
}