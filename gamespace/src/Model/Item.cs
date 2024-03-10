using System;

namespace gamespace.Model;

/// <summary>
/// A callback to the items use method.
/// </summary>
public delegate int ItemUsedCallback(); //Made this string instead of void, for more testable code

/// <summary>
/// A class to represent items inside of the game.
/// </summary>
public class Item
{
    //TODO: Figure out how to draw these in player inventory, and in chests.
    /// <summary>
    /// A readonly value to store the items type.
    /// </summary>
    private readonly ItemType _type;

    /// <summary>
    /// A property to store the items name.
    /// </summary>
    public string ItemName { get; init; }

    /// <summary>
    /// A property to store the items description
    /// </summary>
    public string ItemDescription { get; init; }

    /// <summary>
    /// A boolean to store whether the Item is necessary for game completion.
    /// </summary>
    public bool IsKeyItem { get; init; }

    /// <summary>
    /// A method to store what the item should do when used.
    /// </summary>
    public ItemUsedCallback ItemUse;

    /// <summary>
    /// A field to store who the user of the item is, to be assigned when held.
    /// </summary>
    public Character User { get; set; }

    /// <summary>
    /// An enum to hold what kind of item our object is.
    /// </summary>
    public enum ItemType
    {
        HealingItem,
        TestingItem,
        KeyItem
    }

    /// <summary>
    /// A constructor to create our desired item.
    /// </summary>
    /// <param name="itemName"> Name of the item </param>
    /// <param name="itemDescription"> Description of the item </param>
    /// <param name="isKeyItem"> If our item is a key item for ending the game </param>
    /// <param name="type"> What type of item we are creating </param>
    public Item(string itemName, string itemDescription, bool isKeyItem, ItemType type)
    {
        ItemName = itemName;
        ItemDescription = itemDescription;
        IsKeyItem = isKeyItem;
        _type = type;
    }

    /// <summary>
    /// A callback method for our small potion
    /// </summary>
    /// <returns> returns the desired use method </returns>
    public ItemUsedCallback UseSmallPotion()
    {
        ItemUse = SmallHealPotionUse;
        return SmallHealPotionUse;

        int SmallHealPotionUse() => User.AddHealth(25);
    }

    /// <summary>
    /// A callback method for our medium potion
    /// </summary>
    /// <returns> returns the desired use method </returns>
    public ItemUsedCallback UseMediumPotion()
    {
        ItemUse = MediumHealPotionUse;
        return MediumHealPotionUse;

        int MediumHealPotionUse() => User.AddHealth(50);
    }

    /// <summary>
    /// A callback method for our large potion
    /// </summary>
    /// <returns> returns the desired use method </returns>
    public ItemUsedCallback UseLargePotion()
    {
        ItemUse = LargeHealPotionUse;
        return LargeHealPotionUse;

        int LargeHealPotionUse() => User.AddHealth(75);
    }

    /// <summary>
    /// A callback method to be used for testing that is to be deleted before launch.
    /// </summary>
    /// <returns> returns a string to test against. </returns>
    public int UseTestItem()
    {
        ItemUse = LargeHealPotionUse;
        return LargeHealPotionUse();
        
        int LargeHealPotionUse() => 1;
    }

    /// <summary>
    /// Provides a string representation of Item.
    /// </summary>
    /// <returns> A string representation of the item </returns>
    public override string ToString()
    {
        var itemInfo = "Name: " + ItemName + " Item Description: "
                       + ItemDescription + " Item Type: " + _type;
        return itemInfo;
    }
}