using gamespace.Model;
using gamespace.View;
using Microsoft.Xna.Framework;

namespace gamespace.Managers;

public class WorldBuilder
{
    private World _world;
    private Camera _camera;
    private GameManager _gm;

    public WorldBuilder(GameManager gm, Camera camera, World world)
    {
        _gm = gm;
        _camera = camera;
        _world = world;
    }
    
    public void MakeRoom()
    {
        var width = 5; //This does work with any static width and height
        var height = 5;
        for (var k = 0; k < width + 2; k++)
        {
            _world.CurrentPos += new Vector2(1, 0);
            _world.TryPlaceTile(new Point((int)_world.CurrentPos.X, (int)_world.CurrentPos.Y), BuildTile(_world.CurrentPos, Build.Props.Wall));
            //_world.CurrentPos += new Vector2(1, 0);
        }
        _world.CurrentPos += new Vector2(-width - 1, 1);
        for (var i = 0; i < height; i++)
        {
            //Place one wall on left
            if (i != height / 2 && i != (height / 2 + 1))
            {
                _world.TryPlaceTile(new Point((int)_world.CurrentPos.X, (int)_world.CurrentPos.Y),
                BuildTile(_world.CurrentPos, Build.Props.Wall));
            }

            for (var j = 0; j < width; j++)
            {
                //Place floors
                _world.CurrentPos += new Vector2(1, 0);
                _world.TryPlaceTile(new Point((int)_world.CurrentPos.X, (int)_world.CurrentPos.Y), BuildTile(_world.CurrentPos, Build.Props.Floor));
            }
            _world.CurrentPos += new Vector2(1, 0);
            _world.TryPlaceTile(new Point((int)_world.CurrentPos.X, (int)_world.CurrentPos.Y), BuildTile(_world.CurrentPos, Build.Props.Wall));
            _world.CurrentPos += new Vector2(-width - 1, 1);
            //Place one wall on right
        }
        for (var k = 0; k < width + 2; k++)
        {
            //_world.CurrentPos += new Vector2(1, 0);
            _world.TryPlaceTile(new Point((int)_world.CurrentPos.X, (int)_world.CurrentPos.Y), BuildTile(_world.CurrentPos, Build.Props.Wall));
            _world.CurrentPos += new Vector2(1, 0);
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