namespace gamespace.Model;

public class Tile
{
    private int _x;
    private int _y;
    private Prop[] _props;
    private int _roomID;

    public Tile(int x, int y, Prop[] props, int roomId)
    {
        _x = x;
        _y = y;
        _props = props;
        _roomID = roomId;
    }

    public bool HasProp()
    {
        //TODO: Implementation for checking if the tile has props
        return false;
    }

    public Prop[] GetProps()
    {
        //TODO: Implementation for getting props on the tile
        return null;
    }
}