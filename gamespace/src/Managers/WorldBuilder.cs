using System;
using gamespace.Model;
using gamespace.View;
using Microsoft.Xna.Framework;
using Point = System.Drawing.Point;

namespace gamespace.Managers;

public class WorldBuilder
{
    private readonly World _world;
    private readonly Camera _camera;
    private readonly GameManager _gm;
    private const int AttemptsToPlaceRoom = 100;
    private const int RoomLowerBound = 5;
    private const int RoomUpperBound = 15;
    private int _currentRoomWidth = 5;
    private int _currentRoomHeight = 5;
    private Vector2 _randomDirection = new();
    private Vector2 _currentDirection = new();

    private static readonly Vector2 MoveRight = new(1, 0);
    private static readonly Vector2 MoveLeft = new(-1, 0);
    private static readonly Vector2 MoveUp = new(0, -1);
    private static readonly Vector2 MoveDown = new(0, 1);
    private static readonly Vector2[] Directions = { MoveRight, MoveUp, MoveLeft, MoveDown };


    private static readonly Random Rand = new();

    public WorldBuilder(GameManager gm, Camera camera, World world)
    {
        _gm = gm;
        _camera = camera;
        _world = world;
    }

    private void MakeRoom()
    {
        var currentAttempts = 0;
        while (true)
        {
            var width = _currentRoomWidth;
            var height = _currentRoomHeight;
            var room = new Rectangle((int)_world.CurrentPos.X, (int)_world.CurrentPos.Y, width + 2, height + 2);
            if (_world.CheckRoomOverlap(room))
            {
                currentAttempts++;
                if (currentAttempts == AttemptsToPlaceRoom)
                {
                    return;
                }

                continue;
            }

            for (var k = 0; k < width + 2; k++)
            {
                _world.CurrentPos += new Vector2(1, 0);
                _world.TryPlaceTile(new Point((int)_world.CurrentPos.X, (int)_world.CurrentPos.Y),
                    BuildTile(_world.CurrentPos, Build.Props.Wall));
            }

            _world.CurrentPos += new Vector2(-width - 1, 1);
            for (var i = 0; i < height; i++)
            {
                if (i != height / 2 && i != (height / 2 - 1))
                {
                    _world.TryPlaceTile(new Point((int)_world.CurrentPos.X, (int)_world.CurrentPos.Y),
                        BuildTile(_world.CurrentPos, Build.Props.Wall));
                }

                for (var j = 0; j < width; j++)
                {
                    _world.CurrentPos += new Vector2(1, 0);
                    _world.TryPlaceTile(new Point((int)_world.CurrentPos.X, (int)_world.CurrentPos.Y),
                        BuildTile(_world.CurrentPos, Build.Props.Floor));
                }

                _world.CurrentPos += new Vector2(1, 0);
                _world.TryPlaceTile(new Point((int)_world.CurrentPos.X, (int)_world.CurrentPos.Y),
                    BuildTile(_world.CurrentPos, Build.Props.Wall));
                _world.CurrentPos += new Vector2(-width - 1, 1);
                //Place one wall on right
            }

            for (var k = 0; k < width + 2; k++)
            {
                _world.TryPlaceTile(new Point((int)_world.CurrentPos.X, (int)_world.CurrentPos.Y),
                    BuildTile(_world.CurrentPos, Build.Props.Wall));
                _world.CurrentPos += new Vector2(1, 0);
            }

            break;
        }
    }

    public void BuildWorld()
    {
        //TODO Make this not hardcoded for min and max X, Y
        MakeRoom();
        for (var i = 0; i < World.NumberOfRooms; i++)
        {
            _currentRoomWidth = Rand.Next(RoomLowerBound, RoomUpperBound);
            _currentRoomHeight = Rand.Next(RoomLowerBound, RoomUpperBound);
            var randX = Rand.Next(-24, 24 - _currentRoomWidth - 2);
            var randY = Rand.Next(-24, 24 - _currentRoomHeight - 2);
            _world.CurrentPos = new Vector2(randX, randY);
            MakeRoom();
        }

        FloodFill();
    }

    private void FloodFill() //Name of method subject to change.
    {
        FillMapWithWalls();
        // (1) Pick a random position not on a floor or wall, and that all adjacent tiles are also not walls , this is tested to work
        PickStartingTile();
        //
        // (2) Pick a random direction, move in this direction until either it randomly decides to change, or that direction * 2 == a floor
        ChooseDirection();
        // 
        //  we can do currentPos += randomDirection for traversal. 
        // (3) Once it determines it hits a dead end, go back up the recursive stack checking at each time a new direction was picked,
        //      if it can go any other directions. How to handle dead ends will be the most complicated part.
        //
        // (4) Once we go back to every turn made, and determine 
    }

    private void PickStartingTile()
    {
        var x = -24; //-24;
        var y = -24;
        while (true)
        {
            if (_world.CheckAdj(new Point(x, y)))
            {
                var pointpos = new Point(x, y);
                var vectorpos = new Vector2(x, y);
                _world.TryPlaceTile(new Point(x, y), BuildTile(new Vector2(x, y), Build.Props.Floor));
                break;
            }

            x++;
        }

        _world.CurrentPos = new Vector2(x, y);
    }

    private void FillMapWithWalls() //Name of method subject to change.
    {
        for (var i = _world._minY; i <= _world._maxY; i++)
        {
            for (var j = _world._minX; j <= _world._maxX; j++)
            {
                if (_world.CheckTileIsNull(j, i))
                {
                    _world.TryPlaceTile(new Point(j, i), BuildTile(new Vector2(j, i), Build.Props.Wall));
                }
            }
        }
    }

    private void ChooseDirection()
    {
        var terminate = 0;
        Vector2 checkNextPos;
        Vector2 checkPos;

        while (true)
        {
            if (_randomDirection.Equals(Vector2.Zero))
            {
                _randomDirection = Directions[Rand.Next(0, 4)];
            }

            checkPos = Vector2.Add(_world.CurrentPos, _randomDirection);
            var temp123 = Vector2.Multiply(_randomDirection, 2);
            checkNextPos = Vector2.Add(_world.CurrentPos, temp123);

            if (!_world.IsInBounds((int)checkNextPos.X, (int)checkNextPos.Y) ||
                !_world.CheckAdj(new Point((int)checkNextPos.X, (int)checkNextPos.Y)) ||
                !_world.CheckAdjWithAvoidance(checkPos,
                    _randomDirection)) //_world.CheckAdjWithAvoidance(checkPos, _randomDirection))
            {
                terminate++;
                if (terminate >= 100)
                {
                    return;
                }

                _randomDirection = Vector2.Zero;
            }
            else
            {
                _currentDirection = _randomDirection;
                _world.CurrentPos += _currentDirection;
                _world.ForcePlaceFloor(_world.CurrentPos, BuildTile(_world.CurrentPos, Build.Props.Floor));
            }
        }
    }

    private void TeleportPoint()
    {
        var x = Rand.Next(-23, 23);
        var y = Rand.Next(-23, 23);
        var counterT = 0;
        while (true)
        {
            if (_world.CheckAdj(new Point(x, y)))
            {
                var pointpos = new Point(x, y);
                var vectorpos = new Vector2(x, y);
                _world.ForcePlaceFloor(vectorpos, BuildTile(new Vector2(x, y), Build.Props.Floor));
                break;
            }

            x = Rand.Next(-23, 23);
            y = Rand.Next(-23, 23);
            counterT++;
            if (counterT == 10)
            {
                break;
            }
        }

        _world.CurrentPos = new Vector2(x, y);
        Console.Out.WriteLine(_world.CurrentPos);
    }
    //=== BUILDER ALIASES ===-------------------------------------------------------------------------------------------

    private Tile BuildTile(Vector2 worldPosition, PropBuilder buildCallback)
    {
        var prop = buildCallback.Invoke(_gm, worldPosition, out var renderable);
        _camera.RegisterRenderable(renderable);
        return new Tile(prop);
    }

    private delegate Prop PropBuilder(GameManager gm, Vector2 worldPosition, out RenderObject renderable);
}