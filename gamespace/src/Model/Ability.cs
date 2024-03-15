namespace gamespace.Model;

public class Ability
{
    /// <summary>
    /// The name of the ability.
    /// </summary>
    public readonly string Name;
    
    /// <summary>
    /// The description of the ability.
    /// </summary>
    public readonly string Desc;
    
    /// <summary>
    /// The callback event on the ability.
    /// </summary>
    public readonly AbilityCallback Callback;

    /// <summary>
    /// Creates an ability object.
    /// </summary>
    /// <param name="name">The name of the object.</param>
    /// <param name="desc">The description of the object.</param>
    /// <param name="callback">The callback event for the ability.</param>
    public Ability(string name, string desc, AbilityCallback callback)
    {
        this.Name = name;
        this.Desc = desc;
        this.Callback = callback;
    }

    /// <summary>
    /// Event/callback for the ability.
    /// </summary>
    public delegate void AbilityCallback();
}