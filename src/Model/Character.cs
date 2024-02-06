using System.Collections.Generic;
using gamespace.View;
using Microsoft.Xna.Framework;

namespace gamespace.Model;

public abstract class Character : Entity
{
    private int _hp;
    private int _maxHp;
    private int _energy;
    private int _maxEnergy;
    private int _baseDmg;
    private List<Ability> _abilities;

    public Character(Vector2 worldCoordinate, int width, int height, int hp, int energy, int baseDmg, World world) :
        base(width, height, world, worldCoordinate)
    {
        _hp = _maxHp = hp;
        _energy = _maxEnergy = energy;
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