using System;
using System.ComponentModel.Design;
using Loyc.Collections;

namespace gamespace.Model;

public class World
{
    /**Sparse list of all map tiles by x, y order  **/
    private AList<AList<Tile>> _tiles = new();

    private readonly int _mapWidth;
    private readonly int _mapHeight;
    private readonly int _minX;
    private readonly int _maxX;
    private readonly int _minY;
    private readonly int _maxY;
    private readonly int _xOffset;
    private readonly int _yOffset;

    /// <summary>
    /// Builds a new World object with the given width and height boundary.
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public World(int width, int height)
    {
        _mapWidth = width;
        _mapHeight = height;
        
        _minX = -_mapWidth / 2;
        _maxX = _xOffset = _mapWidth / 2;
        _minY = -_mapHeight / 2;
        _maxY = _yOffset = _mapHeight / 2;
        
    }

    /// <summary>
    /// Get the tile at the given x,y coordinate.
    /// </summary>
    /// <param name="x">x coordinate</param>
    /// <param name="y">y coordinate</param>
    public Tile this[int x, int y]
    {
        get
        {
            CheckBounds(x, y);
            return _tiles[x + _xOffset / 2][y + _yOffset];
        }
        private set
        {
            CheckBounds(x, y);
            _tiles[x + _xOffset][y + _yOffset] = value;
        }
    }

    /// <summary>
    /// Checks to ensure that the given coordinate is within the world boundary.
    /// </summary>
    /// <param name="x">x coordinate</param>
    /// <param name="y">y coordinate</param>
    /// <exception cref="ArgumentOutOfRangeException">Raise exception if out of bounds.</exception>
    private void CheckBounds(int x, int y)
    {
        if (x < _minX || x > _maxX || y < _minY || y > _maxY)
        {
            throw new ArgumentOutOfRangeException(
                $"({x}, {y}) is out of range ({_minX}..{_maxX}, {_minY}..{_maxY})");
        }
    }

    /// <summary>
    /// Places a tile at the given coordinate if and only if there is no tile present already.
    /// </summary>
    /// <param name="x">x coordinate</param>
    /// <param name="y">y coordinate</param>
    /// <param name="tile">tile object to place at coordinate.</param>
    /// <returns>True if placed, else False.</returns>
    public bool TryPlaceTile(int x, int y, Tile tile)
    {
        CheckBounds(x, y);
        if (this[x, y] != null)
        {
            return false;
        }

        this[x, y] = tile;
        return true;
    }
}