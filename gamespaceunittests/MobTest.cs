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
    
    /// <summary>
    /// A method to test our mobs' constructor.
    /// </summary>
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
    
    /// <summary>
    /// A method to verify our constructor throws an exception when base HP is zero.
    /// </summary>
    [Test]
    public void TestMobConstructorHealthExceptionZero()
    {
        Assert.Throws(typeof(ArithmeticException), ExceptionHelperZero);
    }
    
    /// <summary>
    /// A helper method to generate a mob with 0 HP for our TestMobConstructorHealthExceptionZero() method.
    /// </summary>
    private void ExceptionHelperZero()
    {
        var testMob = new Mob(Vector2.Zero, 0, 100, 10, _testWorld,
            "test", 10, true, true, Mob.MobTypes.Hostile);
    }
    
    /// <summary>
    /// A method to verify our constructor throws an exception when base HP is zero.
    /// </summary>
    [Test]
    public void TestMobConstructorHealthExceptionNegative()
    {
        Assert.Throws(typeof(ArithmeticException), ExceptionHelperNegative);
    }
    
    /// <summary>
    /// A helper method to generate a mob with negative HP for our TestMobConstructorHealthExceptionNegative()
    /// </summary>
    private void ExceptionHelperNegative()
    {
        var testMob = new Mob(Vector2.Zero, -100, 100, 10, _testWorld,
            "test", 10, true, true, Mob.MobTypes.Hostile);
    }
    
    /// <summary>
    /// A test to verify that AddHealth can remove HP from our mob.
    /// </summary>
    [Test]
    public void PlayerRemoveHealthTest()
    {
        var testerMob = new Mob(Vector2.Zero, 100, 100, 10, _testWorld,
            "test", 10, true, true, Mob.MobTypes.Hostile);
        testerMob.AddHealth(-25);
        var expectedData = 75;
        Assert.That(testerMob.Health, Is.EqualTo(expectedData));
    }
    
    /// <summary>
    /// A test to verify that our mob can add an item to the usable inventory.
    /// </summary>
    [Test]
    public void AddInventoryTest()
    {
        _testerMob.AddToInventory(_testerItem);
        var expectedData = new Item[5];
        expectedData[0] = _testerItem;
        Assert.That(_testerMob.Inventory, Is.EqualTo(expectedData));
    }
    
    /// <summary>
    /// A test to verify that our mobs removes and regains HP as expected.
    /// </summary>
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

    /// <summary>
    /// A test to make sure that AddHealth does not allow the mobs' current HP to exceed its' max HP.
    /// </summary>
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
    
    /// <summary>
    /// A test to verify that InventoryUse returns false when the mobs' inventory is empty.
    /// </summary>
    [Test]
    public void UseInventorySlotEmptyTest()
    {
        var testerMob = new Mob(Vector2.Zero, 100, 100, 10,
            _testWorld, "test", 10, true, true,
            Mob.MobTypes.Hostile);
        Assert.That(testerMob.InventoryUse(), Is.EqualTo(false));
    }
    
    /// <summary>
    /// A test to verify that ItemUse works as expected on health potions.
    /// </summary>
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
    
    /// <summary>
    /// A test to verify that AddToInventory returns false when we try to add a key item.
    /// </summary>
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