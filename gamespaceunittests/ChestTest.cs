using gamespace.Managers;
using gamespace.Model;
using Microsoft.Xna.Framework;

namespace gamespaceunittests;

public class ChestTest
{
    [Test]
    public void ChestConstructorTest()
    {
        var testItem = Build.Items.TestItem();
        var testChest = new Chest(Vector2.Zero, 1, 1, true, testItem);
        string expectedData = "World Coordinate: {X:0 Y:0} Width: 1 Height: 1 Has movement: False " +
                                    "Has friction: False Has collision: True" + " Item held: " + testItem.ToString();
        Assert.That(testChest.ToString(), Is.EqualTo(expectedData));
    }

    [Test]
    public void ChestTestInteract()
    {
        var player = new Player("temp", null);
        var testItem = Build.Items.TestItem();
        var testChest = new Chest(Vector2.Zero, 1, 1, true, testItem);
        testChest.InteractWithPlayer(player);
        Assert.That(player.Inventory[0], Is.EqualTo(testItem));
    }
}