using gamespace.Managers;
using gamespace.Model;

namespace gamespaceunittests;

public class PlayerTest
{
    private static World _testWorld = new World(1, 1);
    private Player _testPlayer = new Player("Test", _testWorld);
    private Item _testItem = Build.Items.TestItem();

    [Test]
    public void PlayerConstructorTest()
    {
        var testerPlayer = new Player("Test", _testWorld);
        var expectedData = "Max HP: " + "100" + " Energy: " + "100" +
                           " Base DMG: " + "10" +
                           " Name: " + "Test";
        Assert.That(testerPlayer.ToString(), Is.EqualTo(expectedData));
    }

    [Test]
    public void AddInventoryTest()
    {
        _testPlayer.AddToInventory(_testItem);
        var expectedData = new Item[5];
        expectedData[0] = _testItem;
        Assert.That(_testPlayer.Inventory, Is.EqualTo(expectedData));
    }

    [Test]
    public void UseInventorySlotTest()
    {
        _testPlayer.AddToInventory(_testItem);
        //TODO: Player use slot 0, test method.
    }
}