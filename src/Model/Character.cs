using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace gamespace.Model;

public abstract class Character : Entity
{
    private int _maxHp;
    private int _energy;
    private int _maxEnergy;
    private int _baseDmg;
    private List<Ability> _abilities;

    public int Health { get; protected set; }

    public Character(Vector2 worldCoordinate, int width, int height, int hp, int energy, int baseDmg, World world) :
        base(width, height, world, worldCoordinate)
    {
        Health = _maxHp = hp;
        _energy = _maxEnergy = energy;
        _baseDmg = baseDmg;
    }
}