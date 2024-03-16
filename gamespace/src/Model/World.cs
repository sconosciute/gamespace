using System;
using System.Collections.Generic;
using System.Linq;
using gamespace.Model.Entities;
using gamespace.Model.Props;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;

namespace gamespace.Model;

public class World
{
    /// <summary>
    /// The number of rooms to try and generate.
    /// NOTE: The amount of rooms on the map may not always equal the number of rooms set.
    /// </summary>
    public const int NumberOfRooms = 100;

    /// <summary>
    /// List of all tiles in x,y fashion.
    /// </summary>
    private readonly Dictionary<Vector2, Tile> _tiles = new();

    /// <summary>
    /// Dictionary containing all chests in the world.
    /// </summary>
    public readonly Dictionary<Vector2, Chest> Chests = new();

    /// <summary>
    /// Dictionary containing all spikes in the world.
    /// </summary>
    public readonly Dictionary<Vector2, Spikes> Spikes = new();

    /// <summary>
    /// List of character entities in the world.
    /// </summary>
    public readonly List<Character> Entities = new();

    /// <summary>
    /// List of mob entities in the world.
    /// </summary>
    public readonly List<Mob> Mobs = new();

    /// <summary>
    /// The alter to end the game.
    /// </summary>
    public Altar FinalTileAltar;

    /// <summary>
    /// List of rooms in the world.
    /// </summary>
    public List<Room> Rooms { get; } = new();

    /// <summary>
    /// Min X value of the world.
    /// </summary>
    public readonly int MinX;

    /// <summary>
    /// Max X value of the world.
    /// </summary>
    public readonly int MaxX;

    /// <summary>
    /// Min Y value of the world.
    /// </summary>
    public readonly int MinY;

    /// <summary>
    /// Max Y value of the world.
    /// </summary>
    public readonly int MaxY;

    /// <summary>
    /// X offset.
    /// </summary>
    private readonly int _xOffset;

    /// <summary>
    /// Y offset.
    /// </summary>
    private readonly int _yOffset;

    /// <summary>
    /// Vector for moving right.
    /// </summary>
    private static readonly Vector2 MoveRight = new(1, 0);

    /// <summary>
    /// Vector for moving left.
    /// </summary>
    private static readonly Vector2 MoveLeft = new(-1, 0);

    /// <summary>
    /// Vector for moving up.
    /// </summary>
    private static readonly Vector2 MoveUp = new(0, -1);

    /// <summary>
    /// Vector for moving down.
    /// </summary>
    private static readonly Vector2 MoveDown = new(0, 1);

    /// <summary>
    /// Vector array that stores all directions.
    /// </summary>
    private static readonly Vector2[] Directions = { MoveRight, MoveUp, MoveLeft, MoveDown };

    /// <summary>
    /// Getter for current position.
    /// </summary>
    public Vector2 CurrentPos { get; set; } = new(-3, -3);

    /// <summary>
    /// Builds a new World object with the given width and height boundary.
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public World(int width, int height)
    {
        if (width <= 0 || height <= 0)
        {
            throw new ArithmeticException("Width and height must be a positive non-zero value");
        }

        var mapWidth = width;
        var mapHeight = height;

        MinX = -mapWidth / 2;
        MaxX = _xOffset = mapWidth / 2;
        MinY = -mapHeight / 2;
        MaxY = _yOffset = mapHeight / 2;
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
                $"({x}, {y}) is out of range ({MinX}..{MaxX}, {MinY}..{MaxY})");
        }
    }

    /// <summary>
    /// Checks if a given World coordinate is within the world bounds.
    /// </summary>
    /// <param name="x">World X coordinate</param>
    /// <param name="y">World Y Coordinate</param>
    /// <returns>True if the tile exists within the world boundary else false.</returns>
    public bool IsInBounds(int x, int y) => (x >= MinX && x <= MaxX && y >= MinY && y <= MaxY);

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

    /// <summary>
    /// Force places a tile on the designated spot.
    /// </summary>
    /// <param name="pos">The position to place a floor.</param>
    /// <param name="tile">The type of tile to place.</param>
    public void ForcePlaceFloor(Vector2 pos, Tile tile)
    {
        CheckBounds((int)pos.X, (int)pos.Y);
        this[(int)pos.X, (int)pos.Y] = tile;
    }

    /// <summary>
    /// Checks if rooms overlap each other.
    /// </summary>
    /// <param name="newRoom">The room to check overlap.</param>
    public bool CheckRoomOverlap(Room newRoom)
    {
        if (Rooms.Any(rect => !Rectangle.Intersect(rect.RoomBounds, newRoom.RoomBounds).IsEmpty))
        {
            return true;
        }

        Rooms.Add(newRoom);
        return false;
    }

    /// <summary>
    /// Checks if a coordinate's tile is null or not.
    /// </summary>
    /// <param name="x">X coordinate.</param>
    /// <param name="y">Y coordinate.</param>
    //TODO: Alter how building tiles is managed, to remove the need to use this method.
    public bool CheckTileIsNull(int x, int y)
    {
        var pos = new Point(x, y);
        return this[pos.X, pos.Y] == null;
    }

    /// <summary>
    /// Returns if a targeted position is a floor or not.
    /// </summary>
    /// <param name="pos">The targeted position.</param>
    public bool GetIsFloor(Vector2 pos)
    {
        if (!IsInBounds((int)pos.X, (int)pos.Y))
        {
            return true;
        }

        var value = this[(int)pos.X, (int)pos.Y];
        if (value == null)
        {
            return false;
        }

        return !value.CanCollide;
    }

    public override string ToString()
    {
        return "Min X: " + MinX + " Min Y: " + MinY + " Max X: " + MaxX + " Max Y: " + MaxY;
    }

    //-=ARCHIVE/UNUSED METHODS=-----------------------------------------------------------------------------------------
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

    public bool CheckAdjWithoutDiagonal(Vector2 pos)
    {
        if (!IsInBounds((int)pos.X, (int)pos.Y) || this[(int)pos.X, (int)pos.Y] == null)
        {
            return false;
        }

        return (!this[(int)pos.X + 1, (int)pos.Y].CanCollide || !this[(int)pos.X + 1, (int)pos.Y].CanCollide ||
                !this[(int)pos.X, (int)pos.Y - 1].CanCollide || !this[(int)pos.X, (int)pos.Y + 1].CanCollide);
    }

    public bool CheckAdjWithAvoidance(Point pos, Point lastTile)
    {
        for (var i = pos.X - 1; i <= pos.X + 1; i++)
        {
            for (var j = pos.Y - 1; j <= pos.Y + 1; j++)
            {
                if (IsInBounds((int)i, (int)j))
                {
                    if (this[i, j] == null || i == lastTile.X || j == lastTile.Y) continue;
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
        for (var worldX = MinX; worldX <= MaxX; worldX++)
        {
            for (var worldY = MinY; worldY <= MaxY; worldY++)
            {
                position.X = worldX * 16;
                position.Y = worldY * 16;
                Globals.SpriteBatch.Draw(testTile, position, Color.DarkSlateGray);
            }
        }
    }
}