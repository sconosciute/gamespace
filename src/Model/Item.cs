using System;

namespace gamespace.Model;

public class Item
{
    // Figure out later how to draw these items in inventory.
    private readonly Boolean _canUseOutCombat;
    private readonly String _itemName;
    private readonly String _itemDescription;
    
    public Item(Boolean canUseOutCombat, String itemName, String itemDescription)
    {
        _canUseOutCombat = canUseOutCombat;
        _itemName = itemName;
        _itemDescription = itemDescription;
    }
    public delegate void ItemUsedCallback(); 
    
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