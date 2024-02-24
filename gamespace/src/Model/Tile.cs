using System.Reflection.Metadata.Ecma335;

namespace gamespace.Model;

public class Tile
{

    /// <summary>
    /// Whether the prop on this tile can collide with other objects.
    /// </summary>
    public bool CanCollide => Prop?.CanCollide ?? false;

    public Prop Prop { get; }

    public Tile(Prop prop)
    {
        Prop = prop;
    }
}