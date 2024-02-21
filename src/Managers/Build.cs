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
            //TODO: Discuss how to handle world here, if it is needed and what to do. and what to do with type
            var mob = new Mob(worldPosition, 50, 10, 10, world, "Turret",
                10, Mob.MobTypes.Turret);
            //TODO: Discuss and handle renderable here, if needed.
            renderable = new RenderObject(
                texture: gm.GetTexture(Textures.Turret), //Something like this?
                worldPosition: worldPosition,
                layerDepth: LayerDepth.Background);
            return mob;
        }
    }

    public struct Items
    {
        public static Item SmallHealthPotion(Character user, out ItemUsedCallback useItem)
        {
            var smallPotion = new Item("Small health potion",
                "This will heal small wounds", Item.ItemType.SmallHealthPot);
            useItem = Item.UseSmallPotion(user);
            return smallPotion;
        }
    }
}