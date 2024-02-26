using gamespace.Managers;
using gamespace.Model;
using Assert = NUnit.Framework.Assert;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace gamespaceunittests;

public class MobTest
{
    private readonly World _testWorld = new World(1, 1);
    private Mob _testerMob;
    private Item _testerItem = Build.Items.TestItem();

    [SetUp]
    public void SetUp()
    {
        _testerMob = new Mob(Vector2.Zero, 100, 100, 10,
            _testWorld, "test", 10, true, true,
            Mob.MobTypes.Hostile);
    }

    [Test]
    public void TestMobConstructor()
    {
        var testMob = new Mob(Vector2.Zero, 100, 100, 10, _testWorld,
            "test", 10, true, true, Mob.MobTypes.Hostile);
        var expectedData = "Max HP: " + "100" + " Energy: " + "100" +
                           " Base DMG: " + "10" +
                           " Name: " + "test" + " Type: " + "Hostile" +
                           " HP: " + "100" + " DMG: " + "10" +
                           " can use items: " + "True" + " can move: " + "True";
        Assert.That(testMob.ToString(), Is.EqualTo(expectedData));
    }

    [Test]
    public void TestMobConstructorHealthExceptionZero()
    {
        Assert.Throws(typeof(ArithmeticException), ExceptionHelperZero);
    }

    private void ExceptionHelperZero()
    {
        var testMob = new Mob(Vector2.Zero, 0, 100, 10, _testWorld,
            "test", 10, true, true, Mob.MobTypes.Hostile);
    }

    [Test]
    public void TestMobConstructorHealthExceptionNegative()
    {
        Assert.Throws(typeof(ArithmeticException), ExceptionHelperNegative);
    }

    private void ExceptionHelperNegative()
    {
        var testMob = new Mob(Vector2.Zero, -100, 100, 10, _testWorld,
            "test", 10, true, true, Mob.MobTypes.Hostile);
    }

    [Test]
    public void PlayerRemoveHealthTest()
    {
        var testerMob = new Mob(Vector2.Zero, 100, 100, 10, _testWorld,
            "test", 10, true, true, Mob.MobTypes.Hostile);
        testerMob.AddHealth(-25);
        var expectedData = 75;
        Assert.That(testerMob.Health, Is.EqualTo(expectedData));
    }

    [Test]
    public void AddInventoryTest()
    {
        _testerMob.AddToInventory(_testerItem);
        var expectedData = new Item[5];
        expectedData[0] = _testerItem;
        Assert.That(_testerMob.Inventory, Is.EqualTo(expectedData));
    }

    [Test]
    public void AddHealthTest()
    {
        var testerMob = new Mob(Vector2.Zero, 100, 100, 10, _testWorld,
            "test", 10, true, true, Mob.MobTypes.Hostile);
        testerMob.AddHealth(-50);
        testerMob.AddHealth(25);
        var expectedData = 75;
        Assert.That(testerMob.Health, Is.EqualTo(expectedData));
    }

    [Test]
    public void AddHealthTestExceedsMax()
    {
        var testerMob = new Mob(Vector2.Zero, 100, 100, 10, _testWorld,
            "test", 10, true, true, Mob.MobTypes.Hostile);
        testerMob.AddHealth(-25);
        testerMob.AddHealth(50);
        var expectedData = 100;
        Assert.That(testerMob.Health, Is.EqualTo(expectedData));
    }

    [Test]
    public void UseInventorySlotEmptyTest()
    {
        var testerMob = new Mob(Vector2.Zero, 100, 100, 10,
            _testWorld, "test", 10, true, true,
            Mob.MobTypes.Hostile);
        Assert.That(testerMob.InventoryUse(), Is.EqualTo(false));
    }

    [Test]
    public void TestHealthPotionOnMob()
    {
        var testerMob = new Mob(Vector2.Zero, 100, 100, 10,
            _testWorld, "test", 10, true, true,
            Mob.MobTypes.Hostile);
        var testSmallPot = Build.Items.SmallHealthPotion();
        testerMob.AddToInventory(testSmallPot);
        testerMob.AddHealth(-50);
        testerMob.InventoryUse();

        var expectedData = 75;

        Assert.That(testerMob.Health, Is.EqualTo(expectedData));
    }

    [Test]
    public void AddKeyItemTest()
    {
        var testerMob = new Mob(Vector2.Zero, 100, 100, 10,
            _testWorld, "test", 10, true, true,
            Mob.MobTypes.Hostile);
        var testKeyItem = Build.Items.Cog();
        Assert.That(testerMob.AddToInventory(testKeyItem), Is.EqualTo(false));
    }
}