using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace gamespace.Model;

public class Mob : Character
{
    private const int InventorySize = 5;

    private string _name;
    private bool _canUseItems;
    private bool _canMove;
    private int _hp;
    private int _damage;
    private MobTypes _type;
    private readonly Item[] _inventory;
    public Item[] Inventory => _inventory;

    public enum MobTypes
    {
        Hostile,
        Passive
    }

    //May change this constructor to just take position, and generate rest of the info by Type?
    public Mob(Vector2 worldCoordinate, int hp, int energy, int baseDmg, World world,
        string name, int damage, bool canMove, bool canUseItems, MobTypes type) :
        base(worldCoordinate, 1, 1, hp, energy, baseDmg, world)
    {
        _name = name;
        _hp = hp;
        _damage = damage;
        _canMove = canMove;
        _canUseItems = canUseItems;
        _type = type;
        _inventory = new Item[InventorySize];
    }

    public bool InventoryUse()
    {
        if (_canUseItems == false)
        {
            return false; //This type cannot use items
        }

        var index = Array.IndexOf(_inventory, FirstNonEmpty(_inventory));
        if (index < 0)
        {
            return false; //returns false if inventory is empty
        }

        var wantedItem = _inventory[index];
        var wantedItemUse = wantedItem.ItemUse;
        _inventory[index] = null;
        //wantedItemUse.Invoke();
        wantedItemUse.Invoke();
        return true;
    }

    public bool AddToInventory(Item newItem)
    {
        var firstEmptyIndex = Array.IndexOf(_inventory, null);
        if (firstEmptyIndex < 0)
        {
            return false; //inventory is full
        }

        _inventory[firstEmptyIndex] = newItem;
        return true;
    }

    private static Item FirstNonEmpty(IEnumerable<Item> values)
    {
        return values.FirstOrDefault(item => item != null);
    }
}