using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace gamespace.Model.Entities;

public abstract class Character : Entity
{
    /// <summary>
    /// The max health points of the character.
    /// </summary>
    private readonly int _maxHp;
    
    /// <summary>
    /// The energy of the character.
    /// </summary>
    private readonly int _energy;
    
    /// <summary>
    /// The max energy a character can have.
    /// </summary>
    private readonly int _maxEnergy;
    
    /// <summary>
    /// The base damage of the character.
    /// </summary>
    private readonly int _baseDmg;
    
    /// <summary>
    /// A list containing all abilities of a character.
    /// </summary>
    private List<Ability> _abilities = new();

    /// <summary>
    /// Property to retrieve or set the health of the character.
    /// </summary>
    public int Health { get; private set; }
    
    /// <summary>
    /// Property to retrieve or set the energy of the character.
    /// </summary>
    public int Energy { get; private set; }

    /// <summary>
    /// Initializes a character.
    /// </summary>
    /// <param name="worldCoordinate">The world coordinate the character spawns on.</param>
    /// <param name="width">Width of the character.</param>
    /// <param name="height">Height of the character.</param>
    /// <param name="hp">Health points of the character.</param>
    /// <param name="energy">Energy of the character.</param>
    /// <param name="baseDmg">The base damage the character can do.</param>
    /// <param name="world">The world the character will be drawn in.</param>
    protected Character(Vector2 worldCoordinate, float width, float height, int hp, int energy, int baseDmg, World world) :
        base(width, height, world, worldCoordinate)
    {
        if (hp <= 0)
        {
            throw new ArithmeticException("Max HP cannot be zero or negative");
        }

        Health = _maxHp = hp;
        _energy = _maxEnergy = energy;
        _baseDmg = baseDmg;
    }

    /// <summary>
    /// Adds health to the character.
    /// </summary>
    /// <param name="amount">The amount of health to be added.</param>
    public int AddHealth(int amount) 
    {
        if (Health <= 0)
        {
            return 0;
        }

        if (Health + amount <= _maxHp)
        {
            Health += amount;
        }
        else
        {
            Health = _maxHp;
        }

        Console.Out.WriteLine("healed for: " + amount);

        if (Health <= 0)
        {
            OnDeath(); 
        }

        return Health;
    }

    /// <summary>
    /// ToString method for the stats of the character.
    /// </summary>
    public override string ToString()
    {
        return "Max HP: " + _maxHp + " Energy: " + _energy + " Base DMG: " + _baseDmg;
    }
}