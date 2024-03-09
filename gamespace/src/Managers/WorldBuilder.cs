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
    private const int RoomUpperBound = 13;
    private int _currentRoomWidth = 5;
    private int _currentRoomHeight = 5;
    private Vector2 _randomDirection;
    private Vector2 _currentDirection;
    private Point _lastTile;
    private List<Room> _leftOverRooms;
    private List<Room> _roomsConnectedToStart = new();
    private int _numberOfKeyItemsLeft = 4;
    private int _numberOfRoomsLeft;

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

    private Room MakeRoom()
    {
        var currentAttempts = 0;
        Room room;
        while (true)
        {
            var
                width = _currentRoomWidth; //TODO: Verify this height and width is working as expected, could be why world gen broke.
            var height = _currentRoomHeight;
            Rectangle roomBound = new Rectangle((int)_world.CurrentPos.X, (int)_world.CurrentPos.Y, width, height);
            room = new Room(roomBound);
            /*if (_world.CheckRoomOverlap(room))
            {
                currentAttempts++;
                if (currentAttempts == AttemptsToPlaceRoom)
                {
                    return null;
                }

                continue;
            }
            else
            {
                //_world.Rooms.Add(room);
            }*/

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

            _world.CurrentPos += new Vector2(0, -height);
            break;
        }

        return room;
    }

    /*public void BuildWorld()
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
        _leftOverRooms = _world.Rooms;
        //StartDebugPrint();
        //FillMapWithWalls();
        //ConnectRooms();
        debugFirstTile();
    }*/

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

        //ConnectSingleRoom(startingRoom);

        // After all simple rooms are connected, iterate through left over room list connecting all these rooms through
        //  a brute force tunnel, picking the shortest tunnel found between rooms.
        ///Console.Out.WriteLine();
        // Console.Out.WriteLine();


        //Debug print
        //LeftOverRoomsDebug();


        /*
        var uhOhCounter = 0;
        var index = 0;
        /*foreach (var room in _roomsConnectedToStart)
        {
            _leftOverRooms.Remove(room);
        }

        /*foreach (var room in _leftOverRooms)
        {
            ConnectSingleRoom(room);
        }
        //ConnectIslandRoom(startingRoom);
        var terminateCounter = 0;
        //var uhOhCounter = 0;
        while (true)
        {

            foreach (var room in _world.Rooms)
            {
                if (!room.IsConnectedToStart)
                {
                    ConnectIslandRoom(room);
                    terminateCounter = 0;
                }
                else
                {
                    terminateCounter++;
                }

                if (terminateCounter == _world.Rooms.Count)
                {
                    return;
                }
            }

            if (terminateCounter == _world.Rooms.Count)
            {
                return;
            }
            uhOhCounter++;
            if (uhOhCounter >= 10000)
            {
                break;
            }

        /*uhOhCounter++;
        if (uhOhCounter >= 100)
        {
            break;
        }
        foreach (var room in _roomsConnectedToStart)
        {
            //_leftOverRooms.Remove(room);

        }

        /*foreach (var room in _leftOverRooms)
        {
            ConnectSingleRoom(room);
        }

        ConnectIslandRoom(_leftOverRooms[index]);
        _leftOverRooms.RemoveAt(index);
        }
        StartDebugPrint();
        */
        //LeftOverRoomsDebug();
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
        //Console.Out.WriteLine("New room: --------" + currentRoom);
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
            }

            if (room.RoomBounds.Contains(lastRoomPoint) ||
                room.RoomBounds.Contains(lastRoomPoint with { X = lastRoomPoint.X - 1 }))
            {
                lastRoom = room;
                roomCounter++;
                lastRoomDebugFind = true;
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

                lastRoom.ConnectedRooms.UnionWith(newRoom.ConnectedRooms);
                newRoom.ConnectedRooms.UnionWith(lastRoom.ConnectedRooms);
            }
        }
    }

    private void debugFirstTile()
    {
        foreach (var room in _world.Rooms)
        {
            //ConnectIslandRoom(room);
        }
    }

    /*private void ConnectIslandRoom(Room island)
    {   try
        {
            var rightStartingPoint = island.GetMiddleRight();
            var leftStartingPoint = island.GetMiddleLeft();
            var topStartingPoint = island.GetMiddleTop();
            var bottomStartingPoint = island.GetMiddleBottom();

            //_world.ForcePlaceFloor(rightStartingPoint, BuildTile(rightStartingPoint, Build.Props.Connector));
            _world.ForcePlaceFloor(leftStartingPoint, BuildTile(leftStartingPoint, Build.Props.Connector));
            //_world.ForcePlaceFloor(topStartingPoint, BuildTile(topStartingPoint, Build.Props.Connector));
            //_world.ForcePlaceFloor(bottomStartingPoint, BuildTile(bottomStartingPoint, Build.Props.Connector));

            /*var rightDist = FindDistanceInDirection(island, MoveRight, rightStartingPoint);
            var leftDist = FindDistanceInDirection(island, MoveLeft, leftStartingPoint);
            var topDist = FindDistanceInDirection(island, MoveUp, topStartingPoint);
            var bottomDist = FindDistanceInDirection(island, MoveDown, bottomStartingPoint);
            var distArr = new[] { rightDist, leftDist, topDist, bottomDist };

            var result = distArr.Min();
            if (result == 1000)
            {
                throw new Exception("No Rooms found");
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
        catch (Exception)
        {

        }
    }*/

    /*private int FindDistanceInDirection(Room currentRoom, Vector2 Direction, Vector2 StartingPoint)
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

                    foreach (var room in _world.Rooms)
                    {
                        //room.RoomBounds.Contains(new Point(xPointer + 1, yPointer)) ||
                        if (room.RoomBounds.Contains(new Point((int)currentPos.X, (int)currentPos.Y)))
                        {
                            currentRoom.IsConnectedToStart = room.IsConnectedToStart;
                            return counter;
                        }
                    }
                    currentRoom.IsConnectedToStart = true;
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
            //currentPos = Vector2.Add(currentPos, Direction);
            if (_world.IsInBounds((int)currentPos.X, (int)currentPos.Y))
            {
                _world.ForcePlaceFloor(currentPos, BuildTile(currentPos, Build.Props.Connector));
            }
            else
            {
                throw new Exception("Waka waka");
            }
            currentPos = Vector2.Add(currentPos, Direction);
        }
    }*/

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

    public void BuildBasicWorld()
    {
        //Room firstRoom = null;
        _world.CurrentPos = new Vector2(_world.CurrentPos.X, _world.CurrentPos.Y);
        _currentRoomWidth = 5; 
        _currentRoomHeight = 5;
        
        var firstRoom = MakeRoom();
        _world.Rooms.Add(firstRoom);
        
        for (var i = 0; i < World.NumberOfRooms - 1; i++)
        {
            _currentRoomWidth = 5; 


            _currentRoomHeight = 5; 
            
            _world.CurrentPos = new Vector2(_world.CurrentPos.X + 6, _world.CurrentPos.Y);
            if (!_world.IsInBounds((int)_world.CurrentPos.X + 6, (int)_world.CurrentPos.Y + 6))
            {
                _world.CurrentPos = new Vector2(-24, _world.CurrentPos.Y + 6);
            }
            _world.Rooms.Add(MakeRoom());
            
        }

        _leftOverRooms = _world.Rooms;
        firstRoom.IsConnectedToStart = true;
        FillMapWithWalls();
        ConnectSingleRoom(firstRoom); //In our test this will simply connect everything.
        StartDebugPrint();
        LootAndHazardGenerator();
    }

    private void LootAndHazardGenerator()
    {
        _numberOfRoomsLeft = _world.Rooms.Count - 1; //Subtracting 1 to remove starting room.

        for (var currentRoomIndex = 1; currentRoomIndex < _world.Rooms.Count; currentRoomIndex++)
        {
            //Vector2 PlacePoint = new Vector2(_world.Rooms[currentRoomIndex].RoomBounds.X + 2,
            // _world.Rooms[currentRoomIndex].RoomBounds.Y + 1);
            //Random point in room;


            var randX = Rand.Next(_world.Rooms[currentRoomIndex].RoomBounds.X + 2,
                _world.Rooms[currentRoomIndex].RoomBounds.Right);

            var randY = Rand.Next(_world.Rooms[currentRoomIndex].RoomBounds.Y + 1,
                _world.Rooms[currentRoomIndex].RoomBounds.Bottom - 1);

            Vector2 PlacePoint = new(randX, randY);

            if (DecideToPlaceKeyItem())
            {
                _world.ForcePlaceFloor(PlacePoint,
                    BuildTile(PlacePoint,
                        Build.Props.Wall)); //Let walls be temp key items, connectors be temp mobs/other items
            }
            else
            {
                _world.ForcePlaceFloor(PlacePoint, BuildTile(PlacePoint, Build.Props.Connector));
            }

            _numberOfRoomsLeft--;
        }
    }

    private bool DecideToPlaceKeyItem()
    {
        var percentToPlace = Rand.Next(100);

        var percentForKey = _numberOfKeyItemsLeft / (double)_numberOfRoomsLeft * 100; //TODO: Review this math.

        if (percentForKey >= percentToPlace)
        {
            _numberOfKeyItemsLeft--;
            return true;
        }

        return false;
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

    /*private void ChooseDirection()
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
    }*/

    /*private void PickStartingTile()
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
    }*/

    /*private void TeleportPoint()
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
    }*/

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