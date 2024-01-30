using System.Collections.Generic;

namespace gamespace.Model;


public abstract class Character
{
    private int _hp;
    private int _maxHp;
    private int _energy;
    private int _maxEnergy;
    private int _baseDmg;
    private List<Ability> _abilities;

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