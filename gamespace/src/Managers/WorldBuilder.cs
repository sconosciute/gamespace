using System;
using gamespace.Model;
using gamespace.View;
using Microsoft.Xna.Framework;

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