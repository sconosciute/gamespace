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
        //var _testerItem = Build.Items.TestItem();
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
        //TODO: Set health lower than max, add a number that increase current HP but not over max HP.
    }

    [Test]
    public void AddHealthTestExceedsMax()
    {
        //TODO: Set health lower than max, add a number that increase current HP over max HP.
    }

    [Test]
    public void TestHealthPotionOnMob()
    {
        //TODO: Add a health potion, Set health lower than max, call use item on health potion. Check that health is set correct.
    }
}