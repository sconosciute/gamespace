using gamespace.Managers;
using gamespace.View;
using Microsoft.Xna.Framework;

namespace gamespace.Model;

public class Player : Character
{
    private const int INVENTORY_SIZE = 5;
    
    private string _name;
    private Item[] _inventory;
    public Player(string name, World world)
        : base(Vector2.Zero, 16, 16, 100, 100, 10, world)
    {
        _name = name;
        _inventory = new Item[INVENTORY_SIZE];
    }
    public new void FixedUpdate()
    {
        var direction = InputManager.Direction;
        MoveSpeed = new Vector2(BaseMoveSpeed * direction.X, BaseMoveSpeed * direction.Y);
        
        base.FixedUpdate();
    }

    public bool isInventoryFull()
    {
        return _inventory.Length == INVENTORY_SIZE;
    }

    public void addToInventory(Item newItem)
    {
        if (!isInventoryFull())
        {
            int firstEmptyIndex = System.Array.IndexOf(_inventory, null);
            _inventory[firstEmptyIndex] = newItem;
        }
    }
}