using System;
using System.Collections.Generic;
using System.Linq;
using gamespace.Model;
using gamespace.Util;
using gamespace.View;
using Microsoft.Xna.Framework;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;

namespace gamespace.Managers.BusinessPark;

public class WorldBuilder
{
    /// <summary>
    /// The world object to build on.
    /// </summary>
    private readonly World _world;
    
    /// <summary>
    /// The camera object to render everything built.
    /// </summary>
    private readonly Camera _camera;
    
    /// <summary>
    /// The game manager of the current game.
    /// </summary>
    private readonly GameManager _gm;
    
    /// <summary>
    /// Attempts to place a room before moving onto the next room.
    /// </summary>
    private const int AttemptsToPlaceRoom = 100;
    
    /// <summary>
    /// The lowest amount of rooms that could be generated.
    /// </summary>
    private const int RoomLowerBound = 5;
    
    /// <summary>
    /// The highest amount of rooms that could be generated.
    /// </summary>
    private const int RoomUpperBound = 13;
    
    /// <summary>
    /// Starting room width.
    /// </summary>
    private int _currentRoomWidth = 5;
    
    /// <summary>
    /// Starting room height.
    /// </summary>
    private int _currentRoomHeight = 5;
    
    /// <summary>
    /// List of remaining rooms to be connected.
    /// </summary>
    private List<Room> _leftOverRooms;
    
    /// <summary>
    /// List of rooms that are connected to the starting room.
    /// </summary>
    private readonly List<Room> _roomsConnectedToStart = new();
    
    /// <summary>
    /// The number of key item chests to generate.
    /// </summary>
    private int _numberOfKeyItemsLeft = 4;
    
    /// <summary>
    /// The number of rooms left to be connected.
    /// </summary>
    private int _numberOfRoomsLeft;
    
    /// <summary>
    /// List of all current bullets being displayed in the world.
    /// </summary>
    public readonly List<Projectile> LivingBullets = new();

    /// <summary>
    /// Property of how many bullets needed to iterate through.
    /// </summary>
    public int NumberOfBulletsToIterate { get; set; }
    
    /// <summary>
    /// Property that gives the number of mobs to iterate through.
    /// </summary>
    private int NumberOfMobsToIterate { get; set; }

    /// <summary>
    /// The vector for moving right.
    /// </summary>
    private static readonly Vector2 MoveRight = new(1, 0);
    
    /// <summary>
    /// The vector for moving left.
    /// </summary>
    private static readonly Vector2 MoveLeft = new(-1, 0);
    
    /// <summary>
    /// The vector for moving up.
    /// </summary>
    private static readonly Vector2 MoveUp = new(0, -1);
    
    /// <summary>
    /// The vector for moving down.
    /// </summary>
    private static readonly Vector2 MoveDown = new(0, 1);

    
    /// <summary>
    /// Random variable.
    /// </summary>
    private static readonly Random Rand = new();

    /// <summary>
    /// The main menu that will contain save, load, close, and exit buttons.
    /// </summary>
    /// <param name="gm">The current game.</param>
    /// <param name="camera">The camera to render what is build on the world.</param>
    /// <param name="world">The world to build on.</param>
    public WorldBuilder(GameManager gm, Camera camera, World world)
    {
        _gm = gm;
        _camera = camera;
        _world = world;
    }

    /// <summary>
    /// Creates a single room.
    /// </summary>
    private Room MakeRoom()
    {
        var currentAttempts = 0;
        Room room;
        while (true)
        {
            var width = _currentRoomWidth;
            var height = _currentRoomHeight;
            var roomBound = new Rectangle((int)_world.CurrentPos.X, (int)_world.CurrentPos.Y, width, height);
            room = new Room(roomBound);
            if (_world.CheckRoomOverlap(room))
            {
                currentAttempts++;
                if (currentAttempts == AttemptsToPlaceRoom)
                {
                    return null;
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
            }

            _world.CurrentPos += new Vector2(0, -height);
            break;
        }

        return room;
    }

    /// <summary>
    /// Generates the world.
    /// </summary>
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

            var randX = Rand.Next(_world._minX + 1, _world._maxX - _currentRoomWidth - 2);
            if (randX % 2 == 0)
            {
                randX += 1;
            }

            var randY = Rand.Next(_world._minY + 1, _world._maxY - _currentRoomHeight - 2);
            if (randY % 2 == 0)
            {
                randY += 1;
            }

            _world.CurrentPos = new Vector2(randX, randY);
            MakeRoom();
        }

        _leftOverRooms = _world.Rooms;
        StartDebugPrint();
        LeftOverRoomsDebug();
        FillMapWithWalls();
        ConnectRooms();
        StartDebugPrint();
        LootAndHazardGenerator();
    }

    /// <summary>
    /// Starts the process of connecting all rooms.
    /// </summary>
    private void ConnectRooms()
    {
        var startingRoom = _world.Rooms[0];
        startingRoom.IsConnectedToStart = true;

        ConnectSingleRoom(startingRoom);

        ConnectIslandRoom(startingRoom);

        var uhOhCounter = 0;

        foreach (var room in _leftOverRooms)
        {
            ConnectSingleRoom(room);
        }

        var terminateCounter = 0;
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

            uhOhCounter++;
            if (uhOhCounter >= 100)
            {
                break;
            }
        }
    }


    /// <summary>
    /// Connects a single room to the nearest room.
    /// </summary>
    /// <param name="currentRoom">The room to be connected.</param>
    private void ConnectSingleRoom(Room currentRoom)
    {
        Console.Out.WriteLine();
        int xPointer;
        int yPointer;
        var sameRoom = false;
        for (xPointer = currentRoom.RoomBounds.Left; xPointer <= currentRoom.RoomBounds.Right; xPointer++)
        {
            yPointer = currentRoom.RoomBounds.Top - 1;
            var lastRoomTile = new Point(xPointer, yPointer + 2);
            var nextRoomTile = new Point(xPointer, yPointer - 2);

            if (_world.GetIsFloor(new Vector2(xPointer,
                    yPointer + 1)) &&
                _world.GetIsFloor(new Vector2(xPointer,
                    yPointer - 1)))
            {
                if (sameRoom) continue;
                _world.ForcePlaceFloor(new Vector2(xPointer, yPointer),
                    BuildTile(new Vector2(xPointer, yPointer), Build.Props.Connector));
                FindRoomsSurroundingTile(lastRoomTile, nextRoomTile);
                sameRoom = true;
            }
            else
            {
                sameRoom = false;
            }
        }

        sameRoom = false;
        for (xPointer = currentRoom.RoomBounds.Left; xPointer <= currentRoom.RoomBounds.Right; xPointer++)
        {
            yPointer = currentRoom.RoomBounds.Bottom;
            var lastRoomTile = new Point(xPointer, yPointer - 2);
            var nextRoomTile = new Point(xPointer, yPointer + 2);
            if (_world.GetIsFloor(new Vector2(xPointer, yPointer + 1)) &&
                _world.GetIsFloor(new Vector2(xPointer, yPointer - 1)))
            {
                if (sameRoom) continue;
                _world.ForcePlaceFloor(new Vector2(xPointer, yPointer),
                    BuildTile(new Vector2(xPointer, yPointer), Build.Props.Connector));
                FindRoomsSurroundingTile(lastRoomTile, nextRoomTile);
                sameRoom = true;
            }
            else
            {
                sameRoom = false;
            }
        }

        sameRoom = false;
        for (yPointer = currentRoom.RoomBounds.Top; yPointer <= currentRoom.RoomBounds.Bottom; yPointer++)
        {
            xPointer = currentRoom.RoomBounds.Left;

            var lastRoomTile = new Point(xPointer + 2, yPointer);
            var nextRoomTile = new Point(xPointer - 2, yPointer);

            if (_world.GetIsFloor(new Vector2(xPointer + 1, yPointer)) &&
                _world.GetIsFloor(new Vector2(xPointer - 1, yPointer)))
            {
                if (sameRoom) continue;
                _world.ForcePlaceFloor(new Vector2(xPointer, yPointer),
                    BuildTile(new Vector2(xPointer, yPointer), Build.Props.Connector));
                FindRoomsSurroundingTile(lastRoomTile, nextRoomTile);
                sameRoom = true;
            }
            else
            {
                sameRoom = false;
            }
        }

        sameRoom = false;
        for (yPointer = currentRoom.RoomBounds.Top; yPointer <= currentRoom.RoomBounds.Bottom; yPointer++)
        {
            xPointer = currentRoom.RoomBounds.Right + 1; //+ 1;

            var nextRoomTile = new Point(xPointer + 2, yPointer);
            var lastRoomTile = new Point(xPointer - 2, yPointer);

            if (_world.GetIsFloor(new Vector2(xPointer + 1, yPointer)) &&
                _world.GetIsFloor(new Vector2(xPointer - 1, yPointer)))
            {
                if (sameRoom) continue;
                _world.ForcePlaceFloor(new Vector2(xPointer, yPointer),
                    BuildTile(new Vector2(xPointer, yPointer), Build.Props.Connector));
                FindRoomsSurroundingTile(lastRoomTile, nextRoomTile);
                sameRoom = true;
            }
            else
            {
                sameRoom = false;
            }
        }
    }

    /// <summary>
    /// Branches out from a room and connects them if it runs into a tile that is in another room.
    /// </summary>
    /// <param name="lastRoomPoint">The tile that is within the last room.</param>
    /// <param name="currentRoomPoint">The tile that is within the current room.</param>
    private void FindRoomsSurroundingTile(Point lastRoomPoint, Point currentRoomPoint)
    {
        var roomCounter = 0;
        var newRoom = new Room(new Rectangle());
        var lastRoom = new Room(new Rectangle());
        foreach (var room in _world.Rooms)
        {
            if (room.RoomBounds.Contains(currentRoomPoint) ||
                room.RoomBounds.Contains(currentRoomPoint with { X = currentRoomPoint.X - 1 }))
            {
                newRoom = room;
                roomCounter++;
            }

            if (room.RoomBounds.Contains(lastRoomPoint) ||
                room.RoomBounds.Contains(lastRoomPoint with { X = lastRoomPoint.X - 1 }))
            {
                lastRoom = room;
                roomCounter++;
            }

            if (roomCounter != 2) continue;
            if (newRoom.IsConnectedToStart || lastRoom.IsConnectedToStart)
            {
                newRoom.IsConnectedToStart = true;
                lastRoom.IsConnectedToStart = true;
                newRoom.BeenVisited = true;
                lastRoom.BeenVisited = true;
                if (!_roomsConnectedToStart.Contains(newRoom))
                {
                    _roomsConnectedToStart.Add(newRoom);
                    ConnectSingleRoom(newRoom);
                }

                if (!_roomsConnectedToStart.Contains(lastRoom))
                {
                    _roomsConnectedToStart.Add(lastRoom);
                    ConnectSingleRoom(lastRoom);
                }
            }
            else
            {
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

    /// <summary>
    /// Connects any rooms that did not connect in the main algorithm.
    /// </summary>
    /// <param name="island">The targeted island room.</param>
    private void ConnectIslandRoom(Room island)
    {
        try
        {
            var rightStartingPoint = island.GetMiddleRight();
            var leftStartingPoint = island.GetMiddleLeft();
            var topStartingPoint = island.GetMiddleTop();
            var bottomStartingPoint = island.GetMiddleBottom();

            var rightDist = FindDistanceInDirection(island, MoveRight, rightStartingPoint);
            var leftDist = FindDistanceInDirection(island, MoveLeft, leftStartingPoint);
            var topDist = FindDistanceInDirection(island, MoveUp, topStartingPoint);
            var bottomDist = FindDistanceInDirection(island, MoveDown, bottomStartingPoint);
            var distArr = new[] { rightDist, leftDist, topDist, bottomDist };

            var result = distArr.Min();
            if (result == 1000)
            {
                throw new Exception("No Rooms found");
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

            if (!result.Equals(bottomDist)) throw new Exception("Why are u no work");
            BuildPathInDirection(MoveDown, bottomStartingPoint, result);
        }
        catch (Exception)
        {
            // ignored
        }
    }

    /// <summary>
    /// Finds the distance between rooms in each direction.
    /// </summary>
    /// <param name="currentRoom">The current room.</param>
    /// <param name="direction">The direction to check in.</param>
    /// <param name="startingPoint">The starting point of the current room.</param>
    private int FindDistanceInDirection(Room currentRoom, Vector2 direction, Vector2 startingPoint)
    {
        var counter = 0;
        var currentPos = Vector2.Add(startingPoint, direction);
        while (true)
        {
            currentPos = Vector2.Add(currentPos, direction);
            counter++;
            if (_world.IsInBounds((int)currentPos.X, (int)currentPos.Y))
            {
                if (!_world.GetIsFloor(currentPos)) continue;
                if (counter <= 2)
                {
                    return 1000;
                }

                foreach (var room in _world.Rooms.Where(room => room.RoomBounds.Contains(new Point(
                             (int)currentPos.X + (int)direction.X,
                             (int)currentPos.Y + (int)direction.Y))))
                {
                    currentRoom.IsConnectedToStart = room.IsConnectedToStart;
                    if (currentRoom.ConnectedRooms.Contains(room) || room.ConnectedRooms.Contains(currentRoom))
                    {
                        return 1000;
                    }

                    currentRoom.ConnectedRooms.UnionWith(room.ConnectedRooms);
                    room.ConnectedRooms.UnionWith(currentRoom.ConnectedRooms);
                    return counter;
                }

                currentRoom.IsConnectedToStart = true;
                return counter;
            }
            else
            {
                return 1000;
            }
        }
    }

    /// <summary>
    /// Builds a path given the direction.
    /// </summary>
    /// <param name="direction">The direction to build.</param>
    /// <param name="startingPoint">The starting point of a room.</param>
    /// <param name="numberToPlace">The number of connector tiles to place.</param>
    private void BuildPathInDirection(Vector2 direction, Vector2 startingPoint, int numberToPlace)
    {
        var currentPos = Vector2.Add(startingPoint, direction);
        for (var i = 0; i < numberToPlace; i++)
        {
            if (_world.IsInBounds((int)currentPos.X, (int)currentPos.Y))
            {
                _world.ForcePlaceFloor(currentPos, BuildTile(currentPos, Build.Props.Connector));
            }
            else
            {
                throw new Exception("Waka waka");
            }

            currentPos = Vector2.Add(currentPos, direction);
        }
    }

    /// <summary>
    /// Fills up the map with walls that act as the background.
    /// </summary>
    private void FillMapWithWalls()
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

    /// <summary>
    /// Generates all items or hazards that can generate in rooms.
    /// </summary>
    private void LootAndHazardGenerator()
    {
        _numberOfRoomsLeft = _world.Rooms.Count - 2;

        int randX;
        int randY;
        Vector2 placePoint;
        for (var currentRoomIndex = 1; currentRoomIndex < _world.Rooms.Count - 1; currentRoomIndex++)
        {
            Console.Out.WriteLine("Loot gen rooms left: " + _numberOfRoomsLeft);

            randX = Rand.Next(_world.Rooms[currentRoomIndex].RoomBounds.X + 2,
                _world.Rooms[currentRoomIndex].RoomBounds.Right);

            randY = Rand.Next(_world.Rooms[currentRoomIndex].RoomBounds.Y + 1,
                _world.Rooms[currentRoomIndex].RoomBounds.Bottom - 1);

            placePoint = new Vector2(randX, randY);
            Chest currentChest;
            if (DecideToPlaceKeyItem())
            {
                _world.ForcePlaceFloor(placePoint,
                    BuildChest(placePoint, Build.Items.Cog(),
                        Build.InteractableProps.Chest, out currentChest));
                _world.Chests.Add(placePoint, currentChest);
            }
            else
            {
                var hazardOrChest = Rand.Next(0, 3);
                switch (hazardOrChest)
                {
                    case 0:
                        _world.ForcePlaceFloor(placePoint, BuildChest(placePoint, Build.Items.SmallHealthPotion(),
                            Build.InteractableProps.NormalChest, out currentChest));
                        _world.Chests.Add(placePoint, currentChest);
                        break;
                    case 1:
                        _world.ForcePlaceFloor(placePoint,
                            BuildSpikeTile(placePoint, Build.InteractableProps.Spikes, out var currentSpike));
                        _world.Spikes.Add(placePoint, currentSpike);
                        break;
                    default:
                        BuildMob(placePoint, Build.Mobs.Turret);
                        break;
                }
            }

            ChestDictDebug();
            _numberOfRoomsLeft--;
        }

        randX = Rand.Next(_world.Rooms[^1].RoomBounds.X + 2,
            _world.Rooms[^1].RoomBounds.Right);

        randY = Rand.Next(_world.Rooms[^1].RoomBounds.Y + 1,
            _world.Rooms[^1].RoomBounds.Bottom - 1);
        placePoint = new Vector2(randX, randY);
        _world.ForcePlaceFloor(placePoint, BuildAlter(placePoint, Build.InteractableProps.Alter, out var alter));
        _world.FinalTileAltar = alter;
    }

    /// <summary>
    /// Generates the correct amount of key items on the map.
    /// </summary>
    private bool DecideToPlaceKeyItem()
    {
        var percentToPlace = Rand.Next(100);

        var percentForKey = _numberOfKeyItemsLeft / (double)_numberOfRoomsLeft * 100;

        if (!(percentForKey >= percentToPlace)) return false;
        _numberOfKeyItemsLeft--;
        return true;
    }

    /// <summary>
    /// Unregisters bullets.
    /// </summary>
    public void HandleBulletDeregister(in Guid id)
    {
        for (var i = 0; i < LivingBullets.Count; i++)
        {
            if (!LivingBullets[i].EntityId.Equals(id)) continue;
            LivingBullets.RemoveAt(i);
            NumberOfBulletsToIterate--;
        }
    }

    /// <summary>
    /// Updates the world when the player hit box collides with an interactive item.
    /// </summary>
    /// <param name="player">The main player that interacts with props.</param>
    public void UpdateWorld(Player player)
    {
        for (var i = 0; i < NumberOfBulletsToIterate; i++)
        {
            LivingBullets[i].FixedUpdate();
        }

        for (var i = 0; i < NumberOfMobsToIterate; i++)
        {
            _world.Mobs[i].FixedUpdate();
        }

        var roundedDownPos = new Vector2((float)Math.Floor(player.WorldCoordinate.X),
            (float)Math.Floor(player.WorldCoordinate.Y));
        var roundedUpPos = new Vector2((float)Math.Ceiling(player.WorldCoordinate.X),
            (float)Math.Ceiling(player.WorldCoordinate.Y));
        if (_world.Chests.ContainsKey(roundedDownPos))
        {
            foreach (var item in player.Inventory)
            {
                Console.Write(item + " + ");
            }

            Console.Out.WriteLine();
            _world.Chests[roundedDownPos].InteractWithPlayer(player);
            foreach (var item in player.Inventory)
            {
                Console.Write(item + " + ");
            }

            _world.ForcePlaceFloor(roundedDownPos, BuildTile(roundedDownPos, Build.Props.Connector));
            _world.Chests.Remove(roundedDownPos);
            Console.Out.WriteLine();
        }
        else if (_world.Chests.ContainsKey(roundedUpPos))
        {
            foreach (var item in player.Inventory)
            {
                Console.Write(item + " + ");
            }

            Console.Out.WriteLine();
            _world.Chests[roundedUpPos].InteractWithPlayer(player);
            foreach (var item in player.Inventory)
            {
                Console.Write(item + " + ");
            }

            _world.ForcePlaceFloor(roundedUpPos, BuildTile(roundedUpPos, Build.Props.Connector));
            _world.Chests.Remove(roundedUpPos);
            Console.Out.WriteLine();
        }
        else if (_world.Spikes.TryGetValue(roundedDownPos, out var spike))
        {
            spike.InteractWithPlayer(player);
        }
        else if (_world.Spikes.TryGetValue(roundedUpPos, out spike))
        {
            spike.InteractWithPlayer(player);
        }

        else if (_world.FinalTileAltar.WorldCoordinate == roundedDownPos ||
                 _world.FinalTileAltar.WorldCoordinate == roundedUpPos)
        {
            _world.FinalTileAltar.InteractWithPlayer(player);
        }
    }

    /// <summary>
    /// Sends an event when the mob is shooting.
    /// </summary>
    public event EventHelper.SendMobToWorldBuilder MobShooting;

    /// <summary>
    /// Invokes the event and sends it to game manager.
    /// </summary>
    /// <param name="newMob">The targeted mob.</param>
    private void SendMobToGameManager(in Mob newMob)
    {
        MobShooting?.Invoke(newMob);
    }


    // DEBUGS =========================================================================================================
    
    /// <summary>
    /// Debug method to print out the total amount of rooms.
    /// </summary>
    private void StartDebugPrint()
    {
        var debugCount = 0;
        Console.Out.WriteLine("Total rooms");
        foreach (var room in _world.Rooms)
        {
            debugCount++;
            Console.Out.WriteLine(debugCount + " " + room);
        }

        Console.Out.WriteLine();
    }

    /// <summary>
    /// Debug method that prints out the leftover rooms.
    /// </summary>
    private void LeftOverRoomsDebug()
    {
        var debugCount = 0;
        Console.Out.WriteLine("Left over rooms: ");
        foreach (var room in _leftOverRooms)
        {
            debugCount++;
            Console.Out.WriteLine(debugCount + " " + room);
        }
    }

    /// <summary>
    /// Debug method for checking chests.
    /// </summary>
    private void ChestDictDebug()
    {
        foreach (var chest in _world.Chests)
        {
            Console.Out.WriteLine(chest);
        }
    }


    //=== BUILDER ALIASES ===-------------------------------------------------------------------------------------------

    /// <summary>
    /// Builds a tile at the coordinate given.
    /// </summary>
    /// <param name="worldPosition">The position for the tile to be built.</param>
    /// <param name="buildCallback">The prop builder event.</param>
    private Tile BuildTile(Vector2 worldPosition, PropBuilder buildCallback)
    {
        var prop = buildCallback.Invoke(_gm, worldPosition, out var renderable);
        _camera.RegisterRenderable(renderable);
        return new Tile(prop);
    }

    /// <summary>
    /// Builds a spike at the coordinate given.
    /// </summary>
    /// <param name="worldPosition">The position for the tile to be built.</param>
    /// <param name="buildCallback">The interactive prop builder event.</param>
    /// <param name="prop">The built spike tile.</param>
    private Tile BuildSpikeTile(Vector2 worldPosition, InteractablePropBuilder buildCallback, out Spikes prop)
    {
        prop = buildCallback.Invoke(_gm, worldPosition, out var renderable);
        _camera.RegisterRenderable(renderable);
        return new Tile(prop);
    }

    /// <summary>
    /// Builds an alter at the coordinate given.
    /// </summary>
    /// <param name="worldPosition">The position for the tile to be built.</param>
    /// <param name="buildCallback">The alter prop builder event.</param>
    /// <param name="prop">The built alter tile.</param>
    private Tile BuildAlter(Vector2 worldPosition, AlterBuilder buildCallback, out Altar prop)
    {
        prop = buildCallback.Invoke(_gm, worldPosition, out var renderable);
        _camera.RegisterRenderable(renderable);
        prop.Win += _gm.WinGame;
        return new Tile(prop);
    }

    /// <summary>
    /// Builds a mob at the coordinate given.
    /// </summary>
    /// <param name="worldPosition">The position for the tile to be built.</param>
    /// <param name="buildCallback">The mob builder event.</param>
    private Mob BuildMob(Vector2 worldPosition, MobBuilder buildCallback)
    {
        var newMob = buildCallback.Invoke(_gm, _world, worldPosition, out var renderable);
        _camera.RegisterRenderable(renderable);

        newMob.EntityEvent += renderable.HandleEntityEvent;
        newMob.SendObjToRenderObj += renderable.SendUnrender;
        renderable.Handle += _camera.HandleUnrenderEvent;

        newMob.MobShootEvent += SendMobToGameManager;
        _world.Entites.Add(newMob);
        _world.Mobs.Add(newMob);
        NumberOfMobsToIterate++;

        return newMob;
    }

    /// <summary>
    /// Builds a chest at the coordinate given that contains a single item.
    /// </summary>
    /// <param name="worldPosition">The position for the tile to be built.</param>
    /// <param name="item">The item the chest contains.</param>
    /// <param name="buildCallback">The chest builder event.</param>
    /// <param name="newChest">The built chest.</param>
    private Tile BuildChest(Vector2 worldPosition, Item item, ChestBuilder buildCallback, out Chest newChest)
    {
        newChest = buildCallback.Invoke(_gm, worldPosition, item, out var renderable);
        _camera.RegisterRenderable(renderable);
        return new Tile(newChest);
    }

    /// <summary>
    /// The event to build a chest.
    /// </summary>
    private delegate Chest ChestBuilder(GameManager gm, Vector2 worldPosition, Item item, out RenderObject renderable);

    /// <summary>
    /// The event to build a prop.
    /// </summary>
    private delegate Prop PropBuilder(GameManager gm, Vector2 worldPosition, out RenderObject renderable);

    /// <summary>
    /// The event to build an alter.
    /// </summary>
    private delegate Altar AlterBuilder(GameManager gm, Vector2 worldPosition, out RenderObject renderable);

    /// <summary>
    /// The event to build a spike tile.
    /// </summary>
    private delegate Spikes InteractablePropBuilder(GameManager gm, Vector2 worldPosition, out RenderObject renderable);

    /// <summary>
    /// The event to build a mob.
    /// </summary>
    private delegate Mob MobBuilder(GameManager gm, World world, Vector2 worldPositiona, out RenderObject renderable);

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