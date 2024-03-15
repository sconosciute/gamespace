using System;
using gamespace.Model;
using gamespace.View;
using Microsoft.Xna.Framework;

namespace gamespace.Managers;

/// <summary>
/// Utility class for building Model objects.
/// </summary>
public static class Build
{
    /// <summary>
    /// Structure for props that do not interact with the player.
    /// </summary>
    public struct Props
    {
        /// <summary>
        /// Builds a tile that the player can walk on/through.
        /// </summary>
        public static Prop Floor(GameManager gm, Vector2 worldPosition, out RenderObject renderable)
        {
            var prop = new Prop(worldPosition, 1, 1, false);
            renderable = new RenderObject(
                texture: gm.GetTexture(Textures.TestTile),
                worldPosition: worldPosition,
                layerDepth: Layer.Background);
            return prop;
        }

        /// <summary>
        /// Builds a tile that the player cannot walk through.
        /// </summary>
        public static Prop Wall(GameManager gm, Vector2 worldPosition, out RenderObject renderable)
        {
            var prop = new Prop(worldPosition, 0.9f, 0.9f, true);
            renderable = new RenderObject(
                texture: gm.GetTexture(Textures.Collider),
                worldPosition: worldPosition,
                layerDepth: Layer.Background);
            return prop;
        }

        /// <summary>
        /// Builds the tiles that are used for hallways.
        /// </summary>
        public static Prop Connector(GameManager gm, Vector2 worldPosition, out RenderObject renderable)
        {
            var prop = new Prop(worldPosition, 1, 1, false);
            renderable = new RenderObject(
                texture: gm.GetTexture(Textures.TestTile),
                worldPosition: worldPosition,
                layerDepth: Layer.Midground);
            return prop;
        }
    }

    /// <summary>
    /// Structure for props that interact with the player.
    /// </summary>
    public struct InteractableProps
    {
        /// <summary>
        /// Builds a chest that holds key items that the player can pick up.
        /// </summary>
        public static Chest Chest(GameManager gm, Vector2 worldPosition, Item item, out RenderObject renderable)
        {
            var prop = new Chest(worldPosition, 1, 1, false, item);
            renderable = new RenderObject(
                texture: gm.GetTexture(Textures.Chest),
                worldPosition: worldPosition,
                layerDepth: Layer.Midground);
            return prop;
        }

        /// <summary>
        /// Builds a chest that the player can take an item from.
        /// </summary>
        public static Chest NormalChest(GameManager gm, Vector2 worldPosition, Item item, out RenderObject renderable)
        {
            var prop = new Chest(worldPosition, 1, 1, false, item);
            renderable = new RenderObject(
                texture: gm.GetTexture(Textures.NormalChest),
                worldPosition: worldPosition,
                layerDepth: Layer.Midground);
            return prop;
        }

        /// <summary>
        /// Builds spikes on a tile that will damage the player.
        /// </summary>
        public static Spikes Spikes(GameManager gm, Vector2 worldPosition, out RenderObject renderable)
        {
            var prop = new Spikes(worldPosition, 1, 1, false);
            renderable = new RenderObject(
                texture: gm.GetTexture(Textures.Spike),
                worldPosition: worldPosition,
                layerDepth: Layer.Midground);
            return prop;
        }

        /// <summary>
        /// Builds the alter that the player needs to exit.
        /// </summary>
        public static Altar Alter(GameManager gm, Vector2 worldPosition, out RenderObject renderable)
        {
            var prop = new Altar(worldPosition, 1, 1, false);
            renderable = new RenderObject(
                texture: gm.GetTexture(Textures.RoomConnector),
                worldPosition: worldPosition,
                layerDepth: Layer.Midground);
            return prop;
        }
    }

    /// <summary>
    /// Structure for mobs that the player can damage or be damaged by.
    /// </summary>
    public struct Mobs
    {
        /// <summary>
        /// Builds a stationary turret that will actively shoot at the player.
        /// </summary>
        public static Mob Turret(GameManager gm, World world, Vector2 worldPosition, out RenderObject renderable)
        {
            var mob = new Mob(worldPosition, 50, 10, 10, world, "Turret",
                10, false, false, Mob.MobTypes.Hostile);
            //TODO: Add a Turret Model
            renderable = new RenderObject(
                texture: gm.GetTexture(Textures.Cat),
                worldPosition: worldPosition,
                layerDepth: Layer.Midground,
                entityId: mob.EntityId);

            return mob;
        }

        /// <summary>
        /// Builds a special mob.
        /// </summary>
        public static Mob RogueRanger(GameManager gm, World world, Vector2 worldPosition, Guid id,
            out RenderObject renderable)
        {
            var mob = new Mob(worldPosition, 50, 10, 10, world, "Rogue Ranger",
                10, true, true, Mob.MobTypes.Hostile);
            //TODO: Add a Turret Model
            renderable = new RenderObject(
                texture: null, //gm.GetTexture(Textures.RogueRanger), 
                worldPosition: worldPosition,
                layerDepth: Layer.Midground,
                entityId: id);
            return mob;
        }
    }

    /// <summary>
    /// Structure for only projectiles.
    /// </summary>
    public struct Projectiles
    {
        /// <summary>
        /// Builds a bullet that the player and certain mobs will shoot.
        /// </summary>
        public static Projectile Bullet(GameManager gm, World world, Vector2 worldPosition, Vector2 direction,
            Guid sender, out RenderObject renderable)
        {
            var bullet = new Projectile(0.25f, 0.25f, world, worldPosition, direction, sender);
            renderable = new RenderObject(
                texture: gm.GetTexture(Textures.Bullet),
                worldPosition: worldPosition,
                layerDepth: Layer.Midground,
                entityId: bullet.EntityId);
            return bullet;
        }
    }

    /// <summary>
    /// Structure for items that the player can collect, whether it is a usable item or key item.
    /// </summary>
    public struct Items
    {
        /// <summary>
        /// A small heal potion to heal the player for a small amount.
        /// </summary>
        public static Item SmallHealthPotion()
        {
            var smallPotion = new Item("Small health potion",
                "This will heal small wounds", false, Item.ItemType.HealingItem);
            smallPotion.UseSmallPotion();
            return smallPotion;
        }

        /// <summary>
        /// A medium heal potion that will heal the player for a good amount.
        /// </summary>
        public static Item MediumHealthPotion()
        {
            var mediumPotion = new Item("Medium health potion",
                "This will heal medium wounds", false, Item.ItemType.HealingItem);
            mediumPotion.UseMediumPotion();
            return mediumPotion;
        }

        /// <summary>
        /// A large heal potion that will heal the player tremendously.
        /// </summary>
        public static Item LargeHealthPotion()
        {
            var largePotion = new Item("Large health potion",
                "This will heal severe wounds", false, Item.ItemType.HealingItem);
            largePotion.UseLargePotion();
            return largePotion;
        }

        /// <summary>
        /// Special item that the player needs to collect.
        /// </summary>
        public static Item Cog()
        {
            var cog = new Item("Cog", "This will help you fix the escape pod", true, Item.ItemType.KeyItem);
            return cog;
        }

        /// <summary>
        /// Special item that the player needs to collect.
        /// </summary>
        public static Item Wires()
        {
            var wires = new Item("Wires", "This will help you fix the escape pod", true, Item.ItemType.KeyItem);
            return wires;
        }

        /// <summary>
        /// Special item that the player needs to collect.
        /// </summary>
        public static Item Lever()
        {
            var lever = new Item("Lever", "This will help you fix the escape pod", true, Item.ItemType.KeyItem);
            return lever;
        }

        /// <summary>
        /// Special item that the player needs to collect.
        /// </summary>
        public static Item ControlPanel()
        {
            var controlPanel = new Item("Control panel", "This will help you fix the escape pod", true,
                Item.ItemType.KeyItem);
            return controlPanel;
        }

        /// <summary>
        /// Item used only in unit tests.
        /// </summary>
        public static Item TestItem()
        {
            var testItem = new Item("Test",
                "This item should only be used for testing", false, Item.ItemType.TestingItem);
            testItem.UseTestItem();
            return testItem;
        }
    }
}