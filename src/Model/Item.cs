using System;
using Loyc.Syntax;

namespace gamespace.Model;
public delegate void ItemUsedCallback();
public class Item
{
    // Figure out later how to draw these items in inventory.
    private readonly bool _canUseOutCombat;
    private readonly string _itemName;
    private readonly string _itemDescription;
    private readonly ItemType _type;
    public enum ItemType
    {
        HealthPot = 0
        
    }
    
    
   // May change this to just take Type as constructor and generate rest of the info based off type
    public Item(bool canUseOutCombat, string itemName, string itemDescription, ItemType type)
    {
        _canUseOutCombat = canUseOutCombat;
        _itemName = itemName;
        _itemDescription = itemDescription;
        _type = type;
    }

    public ItemUsedCallback GetItemUse()
    {
        switch (_type)
        {
            case 0 :
                void HealPotionUse() => Console.Write("You have been healed!");
                return HealPotionUse;
            
            default:
                void NoReference() => Console.Write("Cannot find case");
                return NoReference;
        }
    }
    
    //Properties
    public String ItemName
    {
        get => _itemName;
    }
    public String ItemDescription
    {
        get => _itemDescription;
    }
}