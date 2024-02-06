using gamespace.Managers;
using gamespace.View;
using Microsoft.Xna.Framework;

namespace gamespace.Model;

public class Player : Character
{
    private string _name;

    public Player(string name, World world)
        : base(Vector2.Zero, 16, 16, 100, 100, 10, world)
    {
        _name = name;
    }
    public new void FixedUpdate()
    {
        var direction = InputManager.Direction;
        MoveSpeed = new Vector2(BaseMoveSpeed * direction.X, BaseMoveSpeed * direction.Y);
        
        base.FixedUpdate();
    }
}