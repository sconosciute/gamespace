using gamespace.Managers;
using gamespace.View;
using Microsoft.Xna.Framework;

namespace gamespace.Model;

public class Player : Character
{
    private const int InventorySize = 5;
    
    private string _name;
    private Item[] _inventory;
    public Player(string name, World world)
        : base(Vector2.Zero, 1, 1, 100, 100, 10, world)
    {
        _name = name;
        _inventory = new Item[InventorySize];
    }
    public new void FixedUpdate()
    {
        var direction = InputManager.Direction;
        MoveSpeed = new Vector2(BaseMoveSpeed * direction.X, BaseMoveSpeed * direction.Y);
        
        base.FixedUpdate();
    }

    private bool IsInventoryFull()
    {
        return _inventory.Length == InventorySize;
    }

    public void AddToInventory(Item newItem)
    {
        if (!IsInventoryFull())
        {
            var firstEmptyIndex = System.Array.IndexOf(_inventory, null);
            _inventory[firstEmptyIndex] = newItem;
        }
    }
}