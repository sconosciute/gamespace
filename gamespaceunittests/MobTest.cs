using gamespace.Managers;
using gamespace.Model;
using Assert = NUnit.Framework.Assert;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace gamespaceunittests;

public class MobTest
{
    private readonly World _testWorld = new World(1, 1);
    [SetUp]
    public void SetUp()
    {
        var testerMob = new Mob(Vector2.Zero, 100, 100, 10, 
            _testWorld, "test", 10, true, true, 
            Mob.MobTypes.Hostile);
        var testItem = Build.Items.TestItem();
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
    public void InventoryTest()
    {
        
    }
}