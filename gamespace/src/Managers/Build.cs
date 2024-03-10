using gamespace.Model;
using gamespace.View;
using Microsoft.Xna.Framework;

namespace gamespace.Managers;

/// <summary>
/// Utility class for building Model objects.
/// </summary>
public static class Build
{
    public struct Props
    {
        public static Prop Floor(GameManager gm, Vector2 worldPosition, out RenderObject renderable)
        {
            var prop = new Prop(worldPosition, 1, 1, false);
            renderable = new RenderObject(
                texture: gm.GetTexture(Textures.TestTile),
                worldPosition: worldPosition,
                layerDepth: LayerDepth.FloorLevel);
            return prop;
        }

        public static Prop Wall(GameManager gm, Vector2 worldPosition, out RenderObject renderable)
        {
            var prop = new Prop(worldPosition, 0.9f, 0.9f, true);
            renderable = new RenderObject(
                texture: gm.GetTexture(Textures.Collider),
                worldPosition: worldPosition,
                layerDepth: LayerDepth.Background);
            return prop;
        }
        
        public static Prop Connector(GameManager gm, Vector2 worldPosition, out RenderObject renderable)
        {
            var prop = new Prop(worldPosition, 1, 1, false);
            renderable = new RenderObject(
                texture: gm.GetTexture(Textures.RoomConnector),
                worldPosition: worldPosition,
                layerDepth: LayerDepth.FloorLevel);
            return prop;
        }
        
        public static Chest Chest(GameManager gm, Vector2 worldPosition, Item _item, out RenderObject renderable)
        {
            var prop = new Chest(worldPosition, 1, 1, false, _item); //Changed col to false
            renderable = new RenderObject(
                texture: gm.GetTexture(Textures.Chest),
                worldPosition: worldPosition,
                layerDepth: LayerDepth.Midground);
            return prop;
        }
        
        //TODO: Temp
        public static Chest NormalChest(GameManager gm, Vector2 worldPosition, Item _item, out RenderObject renderable)
        {
            var prop = new Chest(worldPosition, 1, 1, false, _item);
            renderable = new RenderObject(
                texture: gm.GetTexture(Textures.NormalChest),
                worldPosition: worldPosition,
                layerDepth: LayerDepth.Midground);
            return prop;
        }
    }

    public struct Mobs
    {
        public static Mob Turret(GameManager gm, World world, Vector2 worldPosition, out RenderObject renderable)
        {
            var mob = new Mob(worldPosition, 50, 10, 10, world, "Turret",
                10, false, false, Mob.MobTypes.Hostile);
            //TODO: Add a Turret Model
            renderable = new RenderObject(
                texture: null, //gm.GetTexture(Textures.Turret), //Something like this?
                worldPosition: worldPosition,
                layerDepth: LayerDepth.Midground);
            return mob;
        }

        public static Mob RogueRanger(GameManager gm, World world, Vector2 worldPosition, out RenderObject renderable)
        {
            var mob = new Mob(worldPosition, 50, 10, 10, world, "Rogue Ranger",
                10, true, true, Mob.MobTypes.Hostile);
            //TODO: Add a Turret Model
            renderable = new RenderObject(
                texture: null, //gm.GetTexture(Textures.RogueRanger), 
                worldPosition: worldPosition,
                layerDepth: LayerDepth.Midground);
            return mob;
        }
    }

    public struct Items
    {
        //TODO: Remove user from here, we will not know the user until after it is set, we could have a get, set that runs on pickup?
        public static Item SmallHealthPotion()
        {
            var smallPotion = new Item("Small health potion",
                "This will heal small wounds", false, Item.ItemType.HealingItem);
            smallPotion.UseSmallPotion();
            // = Item.UseSmallPotion(user);
            return smallPotion;
        }

        public static Item MediumHealthPotion()
        {
            var mediumPotion = new Item("Medium health potion",
                "This will heal medium wounds", false, Item.ItemType.HealingItem);
            mediumPotion.UseMediumPotion();
            return mediumPotion;
        }

        public static Item LargeHealthPotion()
        {
            var largePotion = new Item("Large health potion",
                "This will heal severe wounds", false, Item.ItemType.HealingItem);
            largePotion.UseLargePotion();
            return largePotion;
        }
        //Key Items

        public static Item Cog()
        {
            var cog = new Item("Cog", "This will help you fix the escape pod", true, Item.ItemType.KeyItem);
            return cog;
        }

        public static Item Wires()
        {
            var wires = new Item("Wires", "This will help you fix the escape pod", true, Item.ItemType.KeyItem);
            return wires;
        }

        public static Item Lever()
        {
            var lever = new Item("Lever", "This will help you fix the escape pod", true, Item.ItemType.KeyItem);
            return lever;
        }

        public static Item ControlPanel()
        {
            var controlPanel = new Item("Control panel", "This will help you fix the escape pod", true, Item.ItemType.KeyItem);
            return controlPanel;
        }
        
        //Test item, this can be deleted upon final iteration.
        public static Item TestItem()
        {
            var testItem = new Item("Test",
                "This item should only be used for testing", false, Item.ItemType.TestingItem);
            testItem.UseTestItem();
            return testItem;
        }
    }
}