using System;
using System.Collections.Generic;
using System.Linq;
using gamespace.Model;
using gamespace.View;
using Loyc;
using Loyc.Collections.MutableListExtensionMethods;
using Loyc.Geometry;
using Microsoft.Xna.Framework;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;

namespace gamespace.Managers;

public class WorldBuilder
{
    private readonly World _world;
    private readonly Camera _camera;
    private readonly GameManager _gm;
    private const int AttemptsToPlaceRoom = 100;
    private const int RoomLowerBound = 5;
    private const int RoomUpperBound = 11;
    private int _currentRoomWidth = 5;
    private int _currentRoomHeight = 5;
    private Vector2 _randomDirection;
    private Vector2 _currentDirection;
    private Point _lastTile;
    private List<Room> _leftOverRooms;
    private List<Room> _roomsConnectedToStart = new();

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
        _leftOverRooms = _world.Rooms;
    }

    private void MakeRoom()
    {
        var currentAttempts = 0;
        while (true)
        {
            var width = _currentRoomWidth;
            var height = _currentRoomHeight;
            Rectangle roomBound = new Rectangle((int)_world.CurrentPos.X, (int)_world.CurrentPos.Y, width, height);
            var room = new Room(roomBound);
            if (_world.CheckRoomOverlap(room))
            {
                currentAttempts++;
                if (currentAttempts == AttemptsToPlaceRoom)
                {
                    return;
                }

                continue;
            }

            for (var i = 0; i < height; i++)
            {
                for (var j = 0; j < width; j++)
                {
                    _world.CurrentPos += new Vector2(1, 0);
                    _world.TryPlaceTile(new Point((int)_world.CurrentPos.X, (int)_world.CurrentPos.Y),
                        BuildTile(_world.CurrentPos, Build.Props.Floor));
                }

                _world.CurrentPos += new Vector2(-width, 1);
                //Place one wall on right
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
            if (_currentRoomWidth % 2 == 0)
            {
                _currentRoomWidth += 1;
            }

            _currentRoomHeight = Rand.Next(RoomLowerBound, RoomUpperBound);
            if (_currentRoomHeight % 2 == 0)
            {
                _currentRoomHeight += 1;
            }

            var randX = Rand.Next(-24, 24 - _currentRoomWidth - 2);
            if (randX % 2 == 0)
            {
                randX += 1;
            }

            var randY = Rand.Next(-24, 24 - _currentRoomHeight - 2);
            if (randY % 2 == 0)
            {
                randY += 1;
            }

            _world.CurrentPos = new Vector2(randX, randY);
            MakeRoom();
        }

        StartDebugPrint();
        FillMapWithWalls();
        ConnectRooms();
        //FloodFill();
    }

    private void StartDebugPrint()
    {
        var debugCount = 0;
        foreach (var room in _world.Rooms)
        {
            debugCount++;
            Console.Out.WriteLine(debugCount + " " + room);
        }

        Console.Out.WriteLine();
    }

    private void ConnectRooms()
    {
        //Connect simple rooms first, where there's a room on both sides of a wall.
        //  Add any rooms that do not have a simple connection into left over rooms list,

        //Starting room;
        Room startingRoom = _world.Rooms[0];
        startingRoom.IsConnectedToStart = true;

        ConnectSingleRoom(startingRoom);

        // After all simple rooms are connected, iterate through left over room list connecting all these rooms through
        //  a brute force tunnel, picking the shortest tunnel found between rooms.
        Console.Out.WriteLine();
        Console.Out.WriteLine();
        /*foreach (var room in _roomsConnectedToStart)
        {
            _leftOverRooms.Remove(room);
        }
        */
        //Debug print
        LeftOverRoomsDebug();

        var uhOhCounter = 0;
        var index = 0;
        foreach (var room in _roomsConnectedToStart)
        {
            _leftOverRooms.Remove(room);
        }

        foreach (var room in _leftOverRooms)
        {
            ConnectSingleRoom(room);
        }
        while (_leftOverRooms.Any())
        {
            uhOhCounter++;
            if (uhOhCounter >= 100)
            {
                //break;
            }
            ConnectIslandRoom(_leftOverRooms[index]);
            _leftOverRooms.RemoveAt(index);
            
        }

        LeftOverRoomsDebug();
    }

    private void LeftOverRoomsDebug()
    {
        var debugCount = 0;
        foreach (var room in _leftOverRooms)
        {
            debugCount++;
            Console.Out.WriteLine(debugCount + " " + room);
        }
    }

    private void ConnectSingleRoom(Room currentRoom)
    {
        Console.Out.WriteLine();
        Console.Out.WriteLine("New room: --------" + currentRoom);
        int xPointer;
        int yPointer;
        int counter = 0;
        var sameRoom = false;
        // check along the top
        for (xPointer = currentRoom.RoomBounds.Left; xPointer <= currentRoom.RoomBounds.Right; xPointer++)
        {
            yPointer = currentRoom.RoomBounds.Top - 1;
            var lastRoomTile = new Point(xPointer, yPointer + 2);
            var nextRoomTile = new Point(xPointer, yPointer - 2);

            if (_world.GetIsFloor(new Vector2(xPointer,
                    yPointer + 1)) && //If a connection is found here, set has connection bool to true,
                _world.GetIsFloor(new Vector2(xPointer,
                    yPointer - 1))) // If no connections are found, at the end of the loop,
            {
                //    Add the room into a list to be handled after all easy rooms are connected.
                //counter++;
                if (!sameRoom)
                {
                    _world.ForcePlaceFloor(new Vector2(xPointer, yPointer),
                        BuildTile(new Vector2(xPointer, yPointer), Build.Props.Connector));
                    FindRoomsSurroundingTile(lastRoomTile, nextRoomTile);
                    sameRoom = true;
                }
            }
            else
            {
                sameRoom = false;
            }
        }

        sameRoom = false;
        //check along the bottom
        for (xPointer = currentRoom.RoomBounds.Left; xPointer <= currentRoom.RoomBounds.Right; xPointer++)
        {
            yPointer = currentRoom.RoomBounds.Bottom; //+ 1;
            var lastRoomTile = new Point(xPointer, yPointer - 2);
            var nextRoomTile = new Point(xPointer, yPointer + 2);
            if (_world.GetIsFloor(new Vector2(xPointer, yPointer + 1)) &&
                _world.GetIsFloor(new Vector2(xPointer, yPointer - 1)))
            {
                if (!sameRoom)
                {
                    _world.ForcePlaceFloor(new Vector2(xPointer, yPointer),
                        BuildTile(new Vector2(xPointer, yPointer), Build.Props.Connector));
                    FindRoomsSurroundingTile(lastRoomTile, nextRoomTile);
                    sameRoom = true;
                }
            }
            else
            {
                sameRoom = false;
            }
        }

        sameRoom = false;
        //Check to the left
        for (yPointer = currentRoom.RoomBounds.Top; yPointer <= currentRoom.RoomBounds.Bottom; yPointer++)
        {
            xPointer = currentRoom.RoomBounds.Left; //- 1;

            var lastRoomTile = new Point(xPointer + 2, yPointer);
            var nextRoomTile = new Point(xPointer - 2, yPointer);

            if (_world.GetIsFloor(new Vector2(xPointer + 1, yPointer)) &&
                _world.GetIsFloor(new Vector2(xPointer - 1, yPointer)))
            {
                if (!sameRoom)
                {
                    _world.ForcePlaceFloor(new Vector2(xPointer, yPointer),
                        BuildTile(new Vector2(xPointer, yPointer), Build.Props.Connector));
                    FindRoomsSurroundingTile(lastRoomTile, nextRoomTile);
                    sameRoom = true;
                }
            }
            else
            {
                sameRoom = false;
            }
        }

        sameRoom = false;
        //Check along the right
        for (yPointer = currentRoom.RoomBounds.Top; yPointer <= currentRoom.RoomBounds.Bottom; yPointer++)
        {
            xPointer = currentRoom.RoomBounds.Right + 1; //+ 1;

            var nextRoomTile = new Point(xPointer + 2, yPointer);
            var lastRoomTile = new Point(xPointer - 2, yPointer);

            if (_world.GetIsFloor(new Vector2(xPointer + 1, yPointer)) &&
                _world.GetIsFloor(new Vector2(xPointer - 1, yPointer)))
            {
                if (!sameRoom)
                {
                    _world.ForcePlaceFloor(new Vector2(xPointer, yPointer),
                        BuildTile(new Vector2(xPointer, yPointer), Build.Props.Connector));
                    FindRoomsSurroundingTile(lastRoomTile, nextRoomTile);
                    sameRoom = true;
                }
            }
            else
            {
                sameRoom = false;
            }
        }
    }

    private void FindRoomsSurroundingTile(Point lastRoomPoint, Point currentRoomPoint)
    {
        Console.Out.WriteLine();
        var roomCounter = 0;
        Room newRoom = new Room(new Rectangle());
        Room lastRoom = new Room(new Rectangle());
        bool lastRoomDebugFind = false;
        var nextRoomDebugFind = false;
        foreach (Room room in _world.Rooms)
        {
            //room.RoomBounds.Contains(new Point(xPointer + 1, yPointer)) || 
            if (room.RoomBounds.Contains(currentRoomPoint) ||
                room.RoomBounds.Contains(currentRoomPoint with { X = currentRoomPoint.X - 1 }))
            {
                newRoom = room;
                roomCounter++;
                nextRoomDebugFind = true;
                Console.Out.WriteLine("new room found: " + newRoom);
            }

            if (room.RoomBounds.Contains(lastRoomPoint) ||
                room.RoomBounds.Contains(lastRoomPoint with { X = lastRoomPoint.X - 1 }))
            {
                lastRoom = room;
                roomCounter++;
                lastRoomDebugFind = true;
                Console.Out.WriteLine("last room found: " + lastRoom);
            }

            if (roomCounter == 2)
            {
                //_roomsConnectedToStart.Add(newRoom);
                //newRoom.IsConnectedToStart = currentRoom.IsConnectedToStart;
                if (newRoom.IsConnectedToStart || lastRoom.IsConnectedToStart) //Changed || to ^ (XOR)
                {
                    newRoom.IsConnectedToStart = true;
                    lastRoom.IsConnectedToStart = true;
                    //ConnectSingleRoom(newRoom);
                    if (!_roomsConnectedToStart.Contains(newRoom))
                    {
                        _roomsConnectedToStart.Add(newRoom);
                        //_leftOverRooms.Remove(newRoom);
                        /*if (_leftOverRooms.Contains(newRoom))
                        {
                            _leftOverRooms.Remove(newRoom);
                        }*/
                        ConnectSingleRoom(newRoom);
                    }

                    if (!_roomsConnectedToStart.Contains(lastRoom))
                    {
                        _roomsConnectedToStart.Add(lastRoom);
                        ConnectSingleRoom(lastRoom);
                    }
                    //ConnectSingleRoom(newRoom);
                }
                else
                {
                    //ConnectSingleRoom(lastRoom);
                    if (!lastRoom.BeenVisited)
                    {
                        lastRoom.BeenVisited = true;
                        ConnectSingleRoom(lastRoom);
                    }

                    if (!newRoom.BeenVisited)
                    {
                        newRoom.BeenVisited = true;
                        ConnectSingleRoom(newRoom);
                    }
                }
            }
        }
    }

    private void ConnectIslandRoom(Room island)
    {
        var rightStartingPoint = new Vector2(island.RoomBounds.Right + 2, (island.RoomBounds.Height + 1) / 2);
        var leftStartingPoint = new Vector2(island.RoomBounds.Left - 1, (island.RoomBounds.Height + 1) / 2);
        var topStartingPoint = new Vector2((island.RoomBounds.Width + 1) / 2, island.RoomBounds.Top - 2);
        var bottomStartingPoint = new Vector2((island.RoomBounds.Width + 1) / 2, island.RoomBounds.Bottom + 1);

        var rightDist = FindDistanceInDirection(MoveRight, rightStartingPoint);
        var leftDist = FindDistanceInDirection(MoveLeft, leftStartingPoint);
        var topDist = FindDistanceInDirection(MoveUp, topStartingPoint);
        var bottomDist = FindDistanceInDirection(MoveDown, bottomStartingPoint);
        var distArr = new[] { rightDist, leftDist, topDist, bottomDist };

        var result = distArr.Min();
        if (result == 1000)
        {
            return;
        }
        if (result.Equals(rightDist))
        {
            BuildPathInDirection(MoveRight, rightStartingPoint, result);
            return;
        }
        if (result.Equals(leftDist))
        {
            BuildPathInDirection(MoveLeft, leftStartingPoint, result);
            return;
        }

        if (result.Equals(topDist))
        {
            BuildPathInDirection(MoveUp, topStartingPoint, result);
            return;
        }

        if (result.Equals(bottomDist))
        {
            BuildPathInDirection(MoveDown, bottomStartingPoint, result);
            return;
        }

        throw new Exception("Why are u no work");
    }

    private int FindDistanceInDirection(Vector2 Direction, Vector2 StartingPoint)
    {
        var counter = 0;
        var currentPos = new Vector2(StartingPoint.X, StartingPoint.Y);
        while (true)
        {
            currentPos = Vector2.Add(currentPos, Direction);
            counter++;
            if (_world.IsInBounds((int)currentPos.X, (int)currentPos.Y))
            {
                if (_world.GetIsFloor(currentPos))
                {
                    if (counter <= 1)
                    {
                        return 1000;
                    }
                    return counter;
                }
            }
            else
            {
                return 1000;
            }
        }
    }

    private void BuildPathInDirection(Vector2 Direction, Vector2 StartingPoint, int numberToPlace)
    {
        var currentPos = StartingPoint;
        for (var i = 0; i < numberToPlace; i++)
        {
            currentPos = Vector2.Add(currentPos, Direction);
            if (_world.IsInBounds((int)currentPos.X, (int)currentPos.Y))
            {
                _world.ForcePlaceFloor(currentPos, BuildTile(currentPos, Build.Props.Connector));
            }
            else
            {
                throw new Exception("Waka waka");
            }
        }
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


    //=== BUILDER ALIASES ===-------------------------------------------------------------------------------------------

    private Tile BuildTile(Vector2 worldPosition, PropBuilder buildCallback)
    {
        var prop = buildCallback.Invoke(_gm, worldPosition, out var renderable);
        _camera.RegisterRenderable(renderable);
        return new Tile(prop);
    }

    private delegate Prop PropBuilder(GameManager gm, Vector2 worldPosition, out RenderObject renderable);

    //=== ARCHIVE ===---------------------------------------------------------------------------------------------------
    private void ChooseDirection()
    {
        var terminate = 0;
        var teleport = 0;
        Vector2 checkPos;

        while (true)
        {
            if (teleport >= 10)
            {
                TeleportPoint();
            }

            teleport++;
            if (_randomDirection.Equals(Vector2.Zero))
            {
                while (true)
                {
                    _randomDirection = Directions[Rand.Next(0, 4)];
                    if (!_randomDirection.Equals(-_currentDirection))
                    {
                        break;
                    }
                }
            }

            checkPos = Vector2.Add(_world.CurrentPos, _randomDirection);
            var checkPosPoint = new Point((int)checkPos.X, (int)checkPos.Y);
            if (!_world.CheckAdjWithAvoidance(checkPosPoint, _lastTile))
            {
                // TODO: Discuss possible refactor with terminate variable.
                terminate++;
                if (terminate >= 5000)
                {
                    return;
                }

                _randomDirection = Vector2.Zero;
            }
            else
            {
                teleport = 0;
                _currentDirection = _randomDirection;
                _world.CurrentPos += _currentDirection;
                _world.ForcePlaceFloor(_world.CurrentPos, BuildTile(_world.CurrentPos, Build.Props.Floor));
                _lastTile = new Point((int)_world.CurrentPos.X, (int)_world.CurrentPos.Y);
            }
        }
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
        _lastTile = new Point(x, y);
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

    private void FloodFill() //Name of method subject to change.
    {
        FillMapWithWalls();
        // (1) Pick a random position not on a floor or wall, and that all adjacent tiles are also not walls , this is tested to work
        //PickStartingTile();
        //
        // (2) Pick a random direction, move in this direction until either it randomly decides to change, or that direction * 2 == a floor
        //ChooseDirection();
        // 
        //  we can do currentPos += randomDirection for traversal. 
        // (3) Once it determines it hits a dead end, go back up the recursive stack checking at each time a new direction was picked,
        //      if it can go any other directions. How to handle dead ends will be the most complicated part.
        //
        // (4) Once we go back to every turn made, and determine 
    }
}