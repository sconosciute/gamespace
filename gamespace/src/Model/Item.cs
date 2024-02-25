using System;

namespace gamespace.Model;

public delegate string ItemUsedCallback(); //Made this string instead of void, for more testable code

public class Item
{
    //TODO: Figure out how to draw these in player inventory, and in chests.
    private readonly ItemType _type;
    public string ItemName { get; init; }
    public string ItemDescription { get; init; }

    public ItemUsedCallback ItemUse;
    public Character User { get; set; }

    public enum ItemType
    {
        HealingItem,
        TestingItem
    }

    public Item(string itemName, string itemDescription, ItemType type)
    {
        ItemName = itemName;
        ItemDescription = itemDescription;
        _type = type;
    }

    public ItemUsedCallback UseSmallPotion()
    {
        ItemUse = SmallHealPotionUse;
        return SmallHealPotionUse;
        
        string SmallHealPotionUse() => User.AddHealth(25);
        /*
        user.AddHealth(25); Live version:
        Console.Write("Healed small wounds, 25"); Testing:
        }*/
    }

    public ItemUsedCallback UseMediumPotion()
    {
        ItemUse = MediumHealPotionUse;
        return MediumHealPotionUse;

        string MediumHealPotionUse() => User.AddHealth(50);
        /*{
        User.AddHealth(50); Live version:
        Console.Write("Healed medium wounds, 50"); Testing:
        }*/
    }

    public ItemUsedCallback UseLargePotion()
    {
        ItemUse = LargeHealPotionUse;
        return LargeHealPotionUse;

        string LargeHealPotionUse() => User.AddHealth(75);
        /*{
        User.AddHealth(75); Live version:
        Console.Write("Healed large wounds, 75"); Testing:
        }*/
    }

    public string UseTestItem()
    {
       ItemUse = LargeHealPotionUse;
       return LargeHealPotionUse();

       string LargeHealPotionUse() => "Item works!";
    }

    public override string ToString()
    {
        var itemInfo = "Name: " + ItemName + " Item Description: " 
                       + ItemDescription + " Item Type: " + _type;
        return itemInfo;
    }
}