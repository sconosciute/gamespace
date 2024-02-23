using System;

namespace gamespace.Model;

public delegate void ItemUsedCallback();

public class Item
{
    // Figure out later how to draw these items in inventory.
    private readonly ItemType _type;
    public string ItemName { get; init; }
    public string ItemDescription { get; init; }
    public ItemUsedCallback ItemUse;
    public enum ItemType
    {
        HealingItem
    }

    public Item(string itemName, string itemDescription, ItemType type)
    {
        ItemName = itemName;
        ItemDescription = itemDescription;
        _type = type;
    }

    public ItemUsedCallback UseSmallPotion(Character user)
    {
        ItemUse = SmallHealPotionUse;
        return SmallHealPotionUse;
        void SmallHealPotionUse() => user.AddHealth(25);
    }

    public ItemUsedCallback UseMediumPotion(Character user)
    {
        ItemUse = MediumHealPotionUse;
        return MediumHealPotionUse;
        void MediumHealPotionUse() => user.AddHealth(50);
        /*{
        user.AddHealth(50); Live version:
        Console.Write("Healed large wounds, 50"); Testing:
        }*/
    }

    public ItemUsedCallback UseLargePotion(Character user)
    {
        ItemUse = LargeHealPotionUse;
        return LargeHealPotionUse;
        void LargeHealPotionUse() => user.AddHealth(75);
        /*{
        user.AddHealth(75); Live version:
        Console.Write("Healed large wounds, 75"); Testing:
        }*/
    }
    //Properties

    public override string ToString()
    {
        return ItemName;
    }
}