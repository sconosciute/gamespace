using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;

namespace gamespace.Model;

public class World
{
    public const int NumberOfRooms = 400;

    /**Sparse list of all map tiles by x, y order  **/
    private readonly Dictionary<Vector2, Tile> _tiles = new();

    public List<Room> Rooms { get; } = new();

    //Mins, maxes, and offsets need to be accessed repeatedly, caching rather than calculating.

    //TODO: Change this to use properties
    public readonly int _minX;
    public readonly int _maxX;
    public readonly int _minY;
    public readonly int _maxY;
    private readonly int _xOffset;
    private readonly int _yOffset;

    //Adding these for prototyping World Gen
    private static readonly Vector2 MoveRight = new(1, 0);
    private static readonly Vector2 MoveLeft = new(-1, 0);
    private static readonly Vector2 MoveUp = new(0, -1);
    private static readonly Vector2 MoveDown = new(0, 1);
    private static readonly Vector2[] Directions = { MoveRight, MoveUp, MoveLeft, MoveDown };

    //TODO: This property will likely be removed before completion
    public Vector2 CurrentPos { get; set; } = new(-24, -23); //changed -3 to 5

    /// <summary>
    /// Builds a new World object with the given width and height boundary.
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public World(int width, int height)
    {
        var mapWidth = width;
        var mapHeight = height;

        _minX = -mapWidth / 2;
        _maxX = _xOffset = mapWidth / 2;
        _minY = -mapHeight / 2;
        _maxY = _yOffset = mapHeight / 2;
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
    public bool IsInBounds(int x, int y) => (x >= _minX && x <= _maxX && y >= _minY && y <= _maxY);

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

    public void ForcePlaceFloor(Vector2 pos, Tile tile)
    {
        CheckBounds((int)pos.X, (int)pos.Y);
        this[(int)pos.X, (int)pos.Y] = tile;
    }

    public bool CheckRoomOverlap(Room newRoom)
    {
        foreach (var rect in Rooms)
        {
            if (!Rectangle.Intersect(rect.RoomBounds, newRoom.RoomBounds).IsEmpty)
            {
                return true;
            }
        }

        Rooms.Add(newRoom);
        return false;
    }


    //TODO: Alter how building tiles is managed, to remove the need to use this method.
    public bool CheckTileIsNull(int x, int y)
    {
        Point pos = new Point(x, y);
        if (this[pos.X, pos.Y] != null)
        {
            return false;
        }

        return true;
    }

    public bool GetIsFloor(Vector2 pos)
    {
        if (!IsInBounds((int)pos.X, (int)pos.Y))
        {
            return true; //This is out of bounds.
        }

        var value = this[(int)pos.X, (int)pos.Y];
        if (value == null)
        {
            return false;
        }

        return !value.CanCollide; //Should only be called when there's no empty tiles.
    }

    public bool CheckAdj(Point pos)
    {
        for (var i = pos.X - 1; i <= pos.X + 1; i++)
        {
            for (var j = pos.Y - 1; j <= pos.Y + 1; j++)
            {
                if (!IsInBounds(i, j) || this[i, j] == null || i == pos.X || j == pos.Y) continue;
                if (!this[i, j].CanCollide)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public bool CheckAdjWithAvoidance(Point pos, Point lastTile)
    {
        for (var i = pos.X - 1; i <= pos.X + 1; i++)
        {
            for (var j = pos.Y - 1; j <= pos.Y + 1; j++)
            {
                if (IsInBounds((int)i, (int)j))
                {
                    //var check = Vector2.Subtract(new Vector2(i, j), currentDir);
                    if (this[i, j] == null || i == lastTile.X || j == lastTile.Y) continue; //pos.X != 0 && pos.Y != 0 && check != CurrentPos)
                    if (!this[i, j].CanCollide)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        return true;
    }


    public void DebugDrawMap()
    {
        var testTile = Globals.Content.Load<Texture2D>("tile");
        var position = new Vector2();
        for (var worldX = _minX; worldX <= _maxX; worldX++)
        {
            for (var worldY = _minY; worldY <= _maxY; worldY++)
            {
                position.X = worldX * 16;
                position.Y = worldY * 16;
                Globals.SpriteBatch.Draw(testTile, position, Color.DarkSlateGray);
            }
        }
    }
}