using System;
using System.Collections.Generic;
using gamespace.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace gamespace.Model;

public class World
{
    public const int NumberOfRooms = 10;
    
    /**Sparse list of all map tiles by x, y order  **/
    private Dictionary<Vector2, Tile> _tiles = new();

    private List<Rectangle> _roomBounds = new List<Rectangle>();

    //Mins, maxes, and offsets need to be accessed repeatedly, caching rather than calculating.
    private readonly int _mapWidth;
    private readonly int _mapHeight;
    private readonly int _minX;
    private readonly int _maxX;
    private readonly int _minY;
    private readonly int _maxY;
    private readonly int _xOffset;
    private readonly int _yOffset;
    
    //Adding these for prototyping World Gen
    private static readonly Vector2 MoveRight = new(1, 0);
    private static readonly Vector2 MoveLeft = new(-1, 0);
    private static readonly Vector2 MoveUp = new(0, -1);
    private static readonly Vector2 MoveDown = new(0, 1);
    private static readonly Vector2[] Directions = { MoveRight, MoveUp, MoveLeft, MoveDown };
    private Vector2 _lastDirection;

    private Vector2 _currentPos = new(0, 0);
    //This property will likely be removed before completion
    public Vector2 CurrentPos
    {
        get { return _currentPos; }
        set { _currentPos = value;  }
    }

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
    /// <returns>Tile at the given coordinate or null if no such tile exists.</returns>
    public Tile this[int x, int y]
    {
        get
        {
            CheckBounds(x, y);
            var key = new Vector2(x + _xOffset, y + _yOffset);
            _tiles.TryGetValue(key, out var tile);
            return tile;
        }
        private set
        {
            CheckBounds(x, y);
            _tiles[new Vector2(x + _xOffset, y + _yOffset)] = value;
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
        if (!IsInBounds(x, y))
        {
            throw new ArgumentOutOfRangeException(
                $"({x}, {y}) is out of range ({_minX}..{_maxX}, {_minY}..{_maxY})");
        }
    }

    /// <summary>
    /// Checks if a given World coordinate is within the world bounds.
    /// </summary>
    /// <param name="x">World X coordinate</param>
    /// <param name="y">World Y Coordinate</param>
    /// <returns>True if the tile exists within the world boundary else false.</returns>
    public bool IsInBounds(int x, int y) => (x > _minX && x < _maxX && y > _minY && y < _maxY);

    /// <summary>
    /// Places a tile at the given coordinate if and only if there is no tile present already.
    /// </summary>
    /// <param name="position">x and y world coordinate.</param>
    /// <param name="tile">tile object to place at coordinate.</param>
    /// <returns>True if placed, else False.</returns>
    public bool TryPlaceTile(Point position, Tile tile)
    {
        CheckBounds(position.X, position.Y);
        if (this[position.X, position.Y] != null)
        {
            return false;
        }

        this[position.X, position.Y] = tile;
        return true;
    }

    public bool CheckRoomOverlap(Rectangle newRoom)
    {
        PrintRoomsDebug();
        foreach (var rect in _roomBounds)
        {
            if (!Rectangle.Intersect(rect, newRoom).IsEmpty)
            {
                return true; 
            }
        }

        _roomBounds.Add(newRoom);
        return false; 
    }

    private void PrintTilesDebug()
    {
        foreach (KeyValuePair<Vector2, Tile> entry in _tiles)
        {
            Console.Out.WriteLine(entry.Key + " " + entry.Value);
        }
    }

    private void PrintRoomsDebug()
    {
        foreach (var rect in _roomBounds)
        {
            Console.Out.WriteLine(rect);
        }
    }
    
    public void DebugDrawMap()
    {
        var testTile = Globals.Content.Load<Texture2D>("tile");
        var position = new Vector2();
        for (int worldX = _minX; worldX <= _maxX; worldX++)
        {
            for (int worldY = _minY; worldY <= _maxY; worldY++)
            {
                position.X = worldX * 16;
                position.Y = worldY * 16;
                Globals.SpriteBatch.Draw(testTile, position, Color.Aquamarine);
            }
        }
    }
    //private delegate Prop PropBuilder(GameManager gm, Vector2 worldPosition, out RenderObject renderable);
}