using System;
using gamespace.Model;
using gamespace.View;
using Microsoft.Xna.Framework;

namespace gamespace.Managers;

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
                layerDepth: LayerDepth.Background);
            return prop;
        }

        public static Prop Wall(GameManager gm, Vector2 worldPosition, out RenderObject renderable)
        {
            var prop = new Prop(worldPosition, 1, 1, true);
            renderable = new RenderObject(
                texture: gm.GetTexture(Textures.Collider),
                worldPosition: worldPosition,
                layerDepth: LayerDepth.Background);
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
                layerDepth: LayerDepth.Background);
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
                layerDepth: LayerDepth.Background);
            return mob;
        }
    }

    public struct Items
    {
        //TODO: Remove user from here, we will not know the user until after it is set, we could have a get, set that runs on pickup?
        public static Item SmallHealthPotion()
        {
            var smallPotion = new Item("Small health potion",
                "This will heal small wounds", Item.ItemType.HealingItem);
            smallPotion.UseSmallPotion();
            // = Item.UseSmallPotion(user);
            return smallPotion;
        }

        public static Item MediumHealthPotion()
        {
            var mediumPotion = new Item("Medium health potion",
                "This will heal medium wounds", Item.ItemType.HealingItem);
            mediumPotion.UseMediumPotion();
            return mediumPotion;
        }

        public static Item LargeHealthPotion()
        {
            var largePotion = new Item("Large health potion",
                "This will heal severe wounds", Item.ItemType.HealingItem);
            largePotion.UseLargePotion();
            return largePotion;
        }
        
    }
}