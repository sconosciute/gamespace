using System;
using System.Collections.Generic;
using System.Linq;
using gamespace.Util;
using Microsoft.Xna.Framework;

namespace gamespace.Model;

/// <summary>
/// A class to create Mobs, NPC's, in our game.
/// </summary>
public class Mob : Character
{
    /// <summary>
    /// A private constant to hold the size of the mobs' inventory.
    /// </summary>
    private const int InventorySize = 5;

    /// <summary>
    /// A string to store the mobs' name.
    /// </summary>
    private string _name;

    /// <summary>
    /// A boolean to store whether the mob can use items.
    /// </summary>
    private bool _canUseItems;

    /// <summary>
    /// A boolean to store whether the mob can move.
    /// </summary>
    private bool _canMove;

    /// <summary>
    /// An integer to store the mobs' health.
    /// </summary>
    private int _hp;

    /// <summary>
    /// An integer to store how much damage the mob deals.
    /// </summary>
    private int _damage;

    /// <summary>
    /// A field to store what type our mob is.
    /// </summary>
    private MobTypes _type;
    
    /// <summary>
    /// An item array to store usable items for our mob.
    /// </summary>
    private readonly Item[] _inventory;
    
    /// <summary>
    /// A public property to access our mobs inventory.
    /// </summary>
    public Item[] Inventory => _inventory;

    private int _counter = 0;
    
    /// <summary>
    /// An enum to store different types of mobs.
    /// </summary>
    public enum MobTypes
    {
        Hostile,
        Passive
    }

    /// <summary>
    /// A constructor to create our mob.
    /// </summary>
    /// <param name="worldCoordinate"> The coordinate in our game the mob should be placed. </param>
    /// <param name="hp"> The max HP our mob should have. </param>
    /// <param name="energy"> The max energy our mob should have. </param>
    /// <param name="baseDmg"> The base damage our mob should do. </param>
    /// <param name="world"> The world our mob should be placed into. </param>
    /// <param name="name"> The name our mob should have. </param>
    /// <param name="damage"> The max damage our mob should do. </param>
    /// <param name="canMove"> Whether or not our mob should be able to move. </param>
    /// <param name="canUseItems"> Whether or not our mob should be able to use items. </param>
    /// <param name="type"> What type our mob should be, hostile or passive. </param>
    public Mob(Vector2 worldCoordinate, int hp, int energy, int baseDmg, World world,
        string name, int damage, bool canMove, bool canUseItems, MobTypes type) :
        base(worldCoordinate, 1f, 1f, hp, energy, baseDmg, world)
    {
        _name = name;
        _hp = hp;
        _damage = damage;
        _canMove = canMove;
        _canUseItems = canUseItems;
        _type = type;
        _inventory = new Item[InventorySize];
    }
    
    /// <summary>
    /// Checks if our mob can use items, and if so uses the first item in the mobs inventory.
    /// </summary>
    /// <returns> returns false if there are no items or items cannot be used, returns true if item is used. </returns>
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
        if (wantedItem == null)
        {
            return false;
        }

        var wantedItemUse = wantedItem.ItemUse;
        _inventory[index] = null;
        wantedItemUse.Invoke();
        return true;
    }
    
    /// <summary>
    /// Adds a new item to the mobs inventory, if it is a useble Item and not a key item.
    /// </summary>
    /// <param name="newItem"> The item to place into our mobs inventory. </param>
    /// <returns></returns>
    public bool AddToInventory(Item newItem)
    {
        if (newItem.IsKeyItem)
        {
            return false; //Mobs should not be able to hold key items.
        }

        var firstEmptyIndex = Array.IndexOf(_inventory, null);
        if (firstEmptyIndex < 0)
        {
            return false;
        }

        newItem.User = this;
        _inventory[firstEmptyIndex] = newItem;
        return true;
    }
    
    /// <summary>
    /// A helper method for our use item to find the first item type in a given array.
    /// </summary>
    /// <param name="values"> the items to iterate through. </param>
    /// <returns></returns>
    private static Item FirstNonEmpty(IEnumerable<Item> values)
    {
        return values.FirstOrDefault(item => item != null);
    }
    
    /// <summary>
    /// Provides a string representation of our mob.
    /// </summary>
    /// <returns> A string representation of the given mob. </returns>
    public override string ToString()
    {
        var result = base.ToString() + " Name: " + _name + " Type: " + _type + " HP: " + _hp + " DMG: " + _damage +
                     " can use items: " + _canUseItems + " can move: " + _canMove;
        return result;
    }

    public event EventHelper.SendMobToWorldBuilder MobShootEvent;

    private void OnMobShootEvent()
    {
        MobShootEvent?.Invoke(this);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        _counter++;
        if (_counter == 5)
        {
            _counter = 0;
            OnMobShootEvent();
        }
    }
}