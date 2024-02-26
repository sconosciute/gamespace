using gamespace.Managers;
using gamespace.Model;
using Microsoft.Xna.Framework;
using Assert = Loyc.MiniTest.Assert;

namespace gamespaceunittests
{
    public class ItemTests
    {
        private readonly Item _testerItem = Build.Items.TestItem();

        private readonly Mob _testerMob = new Mob(Vector2.Zero, 100, 100, 10,
            new World(1, 1), "test", 10, true, true,
            Mob.MobTypes.Hostile);

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestSmallPotionBuild()
        {
            var smallPotion = Build.Items.SmallHealthPotion();
            const string expectedData = "Name: " + "Small health potion" + " Item Description: " +
                                        "This will heal small wounds " + "Item Type: " +
                                        "HealingItem";
            Assert.AreEqual(expectedData, smallPotion.ToString());
        }

        [Test]
        public void TestMediumPotionBuild()
        {
            var mediumPotion = Build.Items.MediumHealthPotion();
            const string expectedData = "Name: " + "Medium health potion" + " Item Description: " +
                                        "This will heal medium wounds " + "Item Type: " +
                                        "HealingItem";
            Assert.AreEqual(expectedData, mediumPotion.ToString());
        }

        [Test]
        public void TestLargePotionBuild()
        {
            var largePotion = Build.Items.LargeHealthPotion();
            const string expectedData = "Name: " + "Large health potion" + " Item Description: " +
                                        "This will heal severe wounds " + "Item Type: " +
                                        "HealingItem";
            Assert.AreEqual(expectedData, largePotion.ToString());
        }

        [Test]
        public void TestBuildingTestItem()
        {
            var testItem = Build.Items.TestItem();
            const string expectedData = "Name: " + "Test" + " Item Description: " +
                                        "This item should only be used for testing " +
                                        "Item Type: " + "TestingItem";

            Assert.AreEqual(expectedData, testItem.ToString());
        }

        [Test]
        public void TestUserChange()
        {
            _testerMob.AddToInventory(_testerItem);
            Assert.AreEqual(_testerMob, _testerItem.User);
        }

        [Test]
        public void TestItemUse()
        {
            var expectedData = "Item works!";
            Assert.AreEqual(expectedData, _testerItem.ItemUse.Invoke());
        }
    }
}