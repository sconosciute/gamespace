using System;

namespace gamespace.Model;
public delegate void ItemUsedCallback();
public class Item
{
    // Figure out later how to draw these items in inventory.
    private readonly ItemType _type;
    public enum ItemType
    {
        SmallHealthPot = 0,
        MediumHealthPot = 1,
        LargeHealthPot = 2,
    }
    
    
   //changing itemName to infer off type, and temporally making description do the same.
    public Item(ItemType type)
    {
        ItemName = type.ToString();
        ItemDescription = type.ToString();
        _type = type;
    }

    public ItemUsedCallback GetItemUse(Character user)
    {
        switch (_type)
        {
            case ItemType.SmallHealthPot :
                return SmallHealPotionUse;
                void SmallHealPotionUse() => user.AddHealth(25);

            case ItemType.MediumHealthPot:
                return MediumHealPotionUse;
                void MediumHealPotionUse() => user.AddHealth(50);

            case ItemType.LargeHealthPot:
                return LargeHealPotionUse;
                void LargeHealPotionUse() => user.AddHealth(75);

            default:
                return NoReference;
                void NoReference() => Console.Write("Cannot find case");
        }
    }
    
    //Properties
    private string ItemName { get; init; }
    public string ItemDescription { get; init; }
    public override string ToString()
    {
            return ItemName;
    }
}