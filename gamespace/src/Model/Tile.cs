using gamespace.Model.Props;

namespace gamespace.Model;

public class Tile
{

    /// <summary>
    /// Whether the prop on this tile can collide with other objects.
    /// </summary>
    public bool CanCollide => Prop?.CanCollide ?? false;

    /// <summary>
    /// Getter for the prop.
    /// </summary>
    public Prop Prop { get; }

    /// <summary>
    /// Creates a tile object.
    /// </summary>
    /// <param name="prop">The prop.</param>
    public Tile(Prop prop)
    {
        Prop = prop;
    }
}