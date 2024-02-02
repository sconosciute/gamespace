using System.Collections.Generic;
using gamespace.View;

namespace gamespace.Model;

public abstract class Character : Entity
{
    private int _hp; 
    private int _maxHp;
    private int _energy;
    private int _maxEnergy;
    private int _baseDmg;
    private List<Ability> _abilities;
    
    public Character(int moveSpeed, RenderObject sprite, int x, int y, int width, int height, bool canCollide,
        bool canMove, int hp, int maxHp, int energy, int maxEnergy, int baseDmg) : base(moveSpeed, sprite, x, y, width, height,canCollide,  canMove)
    {
        _hp = hp;
        _maxHp = maxHp;
        _energy = energy;
        _maxEnergy = maxEnergy;
        _baseDmg = baseDmg;
    }

    public void Harm(int dmg)
    {
        // Implementation for harming the character
    }

    public void Heal(int hp)
    {
        // Implementation for healing the character
    }

    public bool UseAbility(Ability ability)
    {
        // Implementation for using an ability
        return false;
    }
}