using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace gamespace.Model;

public class Mob : Character
{
    private const int InventorySize = 5;
    
    private string _name;
    private int _hp;
    private int _damage;
    private readonly Item[] _inventory;
    
    private enum Types 
    {
        //TODO: add types, unsure what they are as of now
    }
    //May change this constructor to just take position, and generate rest of the info by Type?
    public Mob(Vector2 worldCoordinate, int width, int height, int hp, int energy, int baseDmg, World world, string name, int damage): 
        base(worldCoordinate, width, height, hp, energy, baseDmg, world)
    {
        _name = name;
        _hp = hp;
        _damage = damage;
        _inventory = Inventory = new Item[InventorySize];
    }
    public void InventoryUse()
    {
        var wantedItem = FirstNonEmpty(_inventory);
        var wantedItemUse = wantedItem.GetItemUse();
        wantedItemUse.Invoke();
    }
    public void AddToInventory(Item newItem)
    {
        var firstEmptyIndex  = Array.IndexOf(_inventory, null);
            _inventory[firstEmptyIndex] = newItem;
    }

    private static Item FirstNonEmpty(IEnumerable<Item> values)
    {
        foreach (var item in values)
        {
            if (item != null)
            {
                return item;
            }
        }

        return null;
    }
    //properties
    public Item[] Inventory { get; }
}