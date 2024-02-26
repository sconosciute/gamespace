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
    public void PlayerRemoveHealthTest()
    {
        var testerPlayer = new Player("test", _testWorld);
        testerPlayer.AddHealth(-25);
        var expectedData = 75;
        Assert.That(testerPlayer.Health, Is.EqualTo(expectedData));
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
        var testerPlayer = new Player("test", _testWorld);
        var testSmallPot = Build.Items.SmallHealthPotion();
        testerPlayer.AddToInventory(testSmallPot);
        testerPlayer.AddHealth(-50);
        testerPlayer.InventoryUse(0);

        var expectedData = 75;

        Assert.That(testerPlayer.Health, Is.EqualTo(expectedData));
    }

    [Test]
    public void UseInventorySlotEmptyTest()
    {
        var testerPlayer = new Player("test", _testWorld);
        Assert.That(testerPlayer.InventoryUse(0), Is.EqualTo(false));
    }

    [Test]
    public void AddHealthTest()
    {
        var testerPlayer = new Player("Test", _testWorld);
        var expectedData = 75;
        testerPlayer.AddHealth(-50);
        testerPlayer.AddHealth(25);
        Assert.That(testerPlayer.Health, Is.EqualTo(expectedData));
    }

    [Test]
    public void AddHealthTestExceedsMax()
    {
        var testerPlayer = new Player("Test", _testWorld);
        var expectedData = 100;
        testerPlayer.AddHealth(-25);
        testerPlayer.AddHealth(50);
        Assert.That(testerPlayer.Health, Is.EqualTo(expectedData));
    }
}