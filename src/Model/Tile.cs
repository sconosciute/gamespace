using System.Reflection.Metadata.Ecma335;

namespace gamespace.Model;

public class Tile
{
    private int _x;
    private int _y;

    /// <summary>
    /// Whether the prop on this tile can collide with other objects.
    /// </summary>
    public bool CanCollide => Prop?.CanCollide ?? false;

    public Prop Prop { get; }

    public Tile(int x, int y, Prop prop)
    {
        _x = x;
        _y = y;
        Prop = prop;
    }
}