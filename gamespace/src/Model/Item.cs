using System;

namespace gamespace.Model;

public delegate void ItemUsedCallback();

public class Item
{
    //TODO: Figure out how to draw these in player inventory, and in chests.
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

        void SmallHealPotionUse() => Console.Write("Healed small wounds, 25");
        /*{
        user.AddHealth(25); Live version:
        Console.Write("Healed small wounds, 25"); Testing:
        }*/
    }

    public ItemUsedCallback UseMediumPotion(Character user)
    {
        ItemUse = MediumHealPotionUse;
        return MediumHealPotionUse;

        void MediumHealPotionUse() => Console.Write("Healed medium wounds, 50");
        /*{
        user.AddHealth(50); Live version:
        Console.Write("Healed medium wounds, 50"); Testing:
        }*/
    }

    public ItemUsedCallback UseLargePotion(Character user)
    {
        ItemUse = LargeHealPotionUse;
        return LargeHealPotionUse;

        void LargeHealPotionUse() => Console.Write("Healed large wounds, 75");
        /*{
        user.AddHealth(75); Live version:
        Console.Write("Healed large wounds, 75"); Testing:
        }*/
    }

    public override string ToString()
    {
        var itemInfo = "Name: " + ItemName + " Item Description: " + ItemDescription + " Item Type: " + _type;
        return itemInfo;
    }
}