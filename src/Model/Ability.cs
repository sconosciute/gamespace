namespace gamespace.Model;

public class Ability
{
    public readonly string Name;
    public readonly string Desc;
    public readonly AbilityCallback Callback;

    public Ability(string name, string desc, AbilityCallback callback)
    {
        this.Name = name;
        this.Desc = desc;
        this.Callback = callback;
    }

    public delegate void AbilityCallback();
}