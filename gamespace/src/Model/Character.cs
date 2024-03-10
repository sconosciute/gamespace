﻿using System;
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
    public int Energy { get; protected set; }

    public Character(Vector2 worldCoordinate, float width, float height, int hp, int energy, int baseDmg, World world) :
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

    //Made this for health potion
    public string AddHealth(int amount)
    {
        if (Health + amount <= _maxHp)
        {
            Health += amount;
        }
        else
        {
            Health = _maxHp;
        }
        Console.Out.WriteLine("healed for: " + amount);

        return "Health now: " + Health;
    }

    public override string ToString()
    {
        return "Max HP: " + _maxHp + " Energy: " + _energy + " Base DMG: " + _baseDmg;
    }
}