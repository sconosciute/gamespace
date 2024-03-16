using gamespace.Managers;
using gamespace.Model;
using gamespace.Model.Entities;
using Microsoft.Xna.Framework;
using Assert = Loyc.MiniTest.Assert;

namespace gamespaceunittests
{
    /// <summary>
    /// A testing class for Item.
    /// </summary>
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
        
        /// <summary>
        /// A method to test our small potion build method.
        /// </summary>
        [Test]
        public void TestSmallPotionBuild()
        {
            var smallPotion = Build.Items.SmallHealthPotion();
            const string expectedData = "Name: " + "Small health potion" + " Item Description: " +
                                        "This will heal small wounds " + "Item Type: " +
                                        "HealingItem";
            Assert.AreEqual(expectedData, smallPotion.ToString());
        }
        /// <summary>
        /// A method to test our build medium potion method.
        /// </summary>
        [Test]
        public void TestMediumPotionBuild()
        {
            var mediumPotion = Build.Items.MediumHealthPotion();
            const string expectedData = "Name: " + "Medium health potion" + " Item Description: " +
                                        "This will heal medium wounds " + "Item Type: " +
                                        "HealingItem";
            Assert.AreEqual(expectedData, mediumPotion.ToString());
        }
        
        /// <summary>
        /// A method to test our build large potion method.
        /// </summary>
        [Test]
        public void TestLargePotionBuild()
        {
            var largePotion = Build.Items.LargeHealthPotion();
            const string expectedData = "Name: " + "Large health potion" + " Item Description: " +
                                        "This will heal severe wounds " + "Item Type: " +
                                        "HealingItem";
            Assert.AreEqual(expectedData, largePotion.ToString());
        }
        
        /// <summary>
        /// A method to test our build test item method.
        /// </summary>
        [Test]
        public void TestBuildingTestItem()
        {
            var testItem = Build.Items.TestItem();
            const string expectedData = "Name: " + "Test" + " Item Description: " +
                                        "This item should only be used for testing " +
                                        "Item Type: " + "TestingItem";

            Assert.AreEqual(expectedData, testItem.ToString());
        }
        
        //TODO: Test the key item builders.
        
        /// <summary>
        /// Test if the items user changes when it is added to a characters inventory.
        /// </summary>
        [Test]
        public void TestUserChange()
        {
            _testerMob.AddToInventory(_testerItem);
            Assert.AreEqual(_testerMob, _testerItem.User);
        }
        
        /// <summary>
        /// Tests the itemUse of our test item.
        /// </summary>
        [Test]
        public void TestItemUse()
        {
            var expectedData = 1;
            Assert.AreEqual(expectedData, _testerItem.ItemUse.Invoke());
        }
    }
}