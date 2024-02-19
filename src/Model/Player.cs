using gamespace.Managers;
using gamespace.View;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace gamespace.Model;

public class Player : Character
{
    private Vector2 _position = new(100, 100);
    private readonly float _speed = 200f;
    private readonly AnimationManager _am = new();
    
    private const int InventorySize = 5;
    
    private string _name;
    private Item[] _inventory;
    public Player(string name, World world)
        : base(Vector2.Zero, 16, 16, 100, 100, 10, world)
    {
        _name = name;
        _inventory = new Item[InventorySize];
        
        //TODO: fix passing a Texture2D.
        var playerTexture = Globals.Content.Load<Texture2D>("test");
        _am.AddAnimation(new Vector2(0, 1), new Animation(playerTexture, 4, 8, 0.1f, 1));
        _am.AddAnimation(new Vector2(-1, 0), new Animation(playerTexture, 4, 8, 0.1f, 2));
        _am.AddAnimation(new Vector2(1, 0), new Animation(playerTexture, 4, 8, 0.1f, 3));
        _am.AddAnimation(new Vector2(0, -1), new Animation(playerTexture, 4, 8, 0.1f, 4));
        _am.AddAnimation(new Vector2(-1, 1), new Animation(playerTexture, 4, 8, 0.1f, 5));
        _am.AddAnimation(new Vector2(-1, -1), new Animation(playerTexture, 4, 8, 0.1f, 6));
        _am.AddAnimation(new Vector2(1, 1), new Animation(playerTexture, 4, 8, 0.1f, 7));
        _am.AddAnimation(new Vector2(1, -1), new Animation(playerTexture, 4, 8, 0.1f, 8));
    }
    public new void FixedUpdate()
    {
        if (InputManager.Moving)
        {
            _position += Vector2.Normalize(InputManager.Direction) * _speed * Globals.TotalSeconds;
        }
        
        _am.Update(InputManager.Direction);
        
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
            int firstEmptyIndex = System.Array.IndexOf(_inventory, null);
            _inventory[firstEmptyIndex] = newItem;
        }
    }
}