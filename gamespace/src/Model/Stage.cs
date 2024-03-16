namespace gamespace.Model;

public class Stage
{
    //Unused class - Functionality will be implemented at a later date.
    private readonly Tile[,] _tiles;

    public Stage(int width, int height)
    {
        _tiles = new Tile[width, height];
    }
    public void AddTile(Tile tile, int x, int y)
    {
        _tiles[x, y] = tile;
    }
}