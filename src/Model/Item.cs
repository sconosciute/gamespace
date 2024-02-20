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
    
    
   // May change this to just take Type as constructor and generate rest of the info based off type
   //changing itemName to infer off type, and temporally making description do the same.
    public Item(ItemType type)
    {
        ItemName = type.ToString();
        ItemDescription = type.ToString();
        _type = type;
    }

    public ItemUsedCallback GetItemUse()
    {
        switch (_type)
        {
            case ItemType.SmallHealthPot :
                void SmallHealPotionUse() => Console.Write("You have been slightly healed!");
                return SmallHealPotionUse;

            case ItemType.MediumHealthPot:
                void MediumHealPotionUse() => Console.Write("You have been medium healed!");
                return MediumHealPotionUse;
            
            case ItemType.LargeHealthPot:
                void LargeHealPotionUse() => Console.Write("You have been largely healed!");
                return LargeHealPotionUse;
                
            default:
                return NoReference;
                void NoReference() => Console.Write("Cannot find case");
        }
    }
    
    //Properties
    public String ItemName { get; }
    public String ItemDescription { get; }
    // To String
    public override string? ToString()
    {
            return ItemName;
    }
}