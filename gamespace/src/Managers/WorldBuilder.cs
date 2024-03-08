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
    private const int AttemptsToPlaceRoom = 10000;
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
        FillMapWithWalls();
        ConnectRooms();
        //FloodFill();
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
    }

    private void ConnectSingleRoom(Room currentRoom)
    {
        int xPointer;
        int yPointer;
        int counter = 0;
        // check along the top
         for (xPointer = currentRoom.RoomBounds.Left + 1; xPointer <= currentRoom.RoomBounds.Right; xPointer++)
            {
                yPointer = currentRoom.RoomBounds.Top - 1;
                var lastRoomTile = new Point(xPointer, yPointer + 1);
                var nextRoomTile = new Point(xPointer, yPointer - 1);
                
                if (_world.GetIsFloor(new Vector2(xPointer, yPointer + 1)) &&   //If a connection is found here, set has connection bool to true,
                    _world.GetIsFloor(new Vector2(xPointer, yPointer - 1)))     // If no connections are found, at the end of the loop,
                {                                                                     //    Add the room into a list to be handled after all easy rooms are connected.
                    counter++;
                    _world.ForcePlaceFloor(new Vector2(xPointer, yPointer),
                        BuildTile(new Vector2(xPointer, yPointer), Build.Props.Connector));
                    FindRoomsSurroundingTile(lastRoomTile, nextRoomTile);
                    if (counter == 1)
                    {
                        counter = 0;
                        break; 
                    }
                }
            }
            //check along the bottom
            for (xPointer = currentRoom.RoomBounds.Left + 1; xPointer <= currentRoom.RoomBounds.Right; xPointer++)
            {
                yPointer = currentRoom.RoomBounds.Bottom;
                var lastRoomTile = new Point(xPointer, yPointer - 1);
                var nextRoomTile = new Point(xPointer, yPointer + 1);
                if (_world.GetIsFloor(new Vector2(xPointer, yPointer + 1)) &&
                    _world.GetIsFloor(new Vector2(xPointer, yPointer - 1)))
                {
                    counter++;
                    _world.ForcePlaceFloor(new Vector2(xPointer, yPointer),
                        BuildTile(new Vector2(xPointer, yPointer), Build.Props.Connector));
                    FindRoomsSurroundingTile(lastRoomTile, nextRoomTile);
                    if (counter == 1)
                    {
                        counter = 0;
                        break; 
                    } 
                }
            }
            //Check to the left
            for (yPointer = currentRoom.RoomBounds.Top; yPointer < currentRoom.RoomBounds.Bottom; yPointer++)
            {
                xPointer = currentRoom.RoomBounds.Left;

                var lastRoomTile = new Point(xPointer + 1, yPointer);
                var nextRoomTile = new Point(xPointer - 1, yPointer);
                
                if (_world.GetIsFloor(new Vector2(xPointer + 1, yPointer)) &&
                    _world.GetIsFloor(new Vector2(xPointer - 1, yPointer)))
                {
                    counter++;
                    _world.ForcePlaceFloor(new Vector2(xPointer, yPointer),
                        BuildTile(new Vector2(xPointer, yPointer), Build.Props.Connector));
                    
                    FindRoomsSurroundingTile(lastRoomTile, nextRoomTile); //swaped shoudl be the other way
                    if (counter == 1)
                    {
                        counter = 0;
                        break; 
                    } 
                }
            }
            //Check along the right
            for (yPointer = currentRoom.RoomBounds.Top; yPointer < currentRoom.RoomBounds.Bottom; yPointer++)
            {
                xPointer = currentRoom.RoomBounds.Right + 1;

                var nextRoomTile = new Point(xPointer + 1, yPointer);
                var lastRoomTile = new Point(xPointer - 1, yPointer);
                
                if (_world.GetIsFloor(new Vector2(xPointer + 1, yPointer)) &&
                    _world.GetIsFloor(new Vector2(xPointer - 1, yPointer)))
                {
                    counter++;
                    _world.ForcePlaceFloor(new Vector2(xPointer, yPointer),
                        BuildTile(new Vector2(xPointer, yPointer), Build.Props.Connector));
                    FindRoomsSurroundingTile(lastRoomTile, nextRoomTile);
                    if (counter == 1)
                    {
                        break;
                    }
                }
            }
            
    }

    private void FindRoomsSurroundingTile(Point lastRoomPoint, Point currentRoomPoint)
    {
        var roomCounter = 0;
        Room newRoom = new Room(new Rectangle());
        Room currentRoom = new Room(new Rectangle());
        foreach (Room room in _world.Rooms)
        {
            //room.RoomBounds.Contains(new Point(xPointer + 1, yPointer)) || 
            if (room.RoomBounds.Contains(currentRoomPoint))
            {
                newRoom = room;
                roomCounter++;
            }
            if (room.RoomBounds.Contains(lastRoomPoint))
            {
                currentRoom = room;
                roomCounter++;
            }
                        
            if (roomCounter == 2)
            {
                //_roomsConnectedToStart.Add(newRoom);
                //newRoom.IsConnectedToStart = currentRoom.IsConnectedToStart;
                if (newRoom.IsConnectedToStart || currentRoom.IsConnectedToStart)
                {
                    newRoom.IsConnectedToStart = true;
                    currentRoom.IsConnectedToStart = true;
                    if (!_roomsConnectedToStart.Contains(newRoom))
                    {
                        _roomsConnectedToStart.Add(newRoom);
                        ConnectSingleRoom(newRoom);
                    }
                    if (!_roomsConnectedToStart.Contains(currentRoom))
                    {
                        _roomsConnectedToStart.Add(currentRoom);
                    }
                    //ConnectSingleRoom(newRoom);
                }
                
            }
        }
        //ConnectSingleRoom(newRoom);
        //ConnectSingleRoom(currentRoom);
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