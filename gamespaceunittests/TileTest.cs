using gamespace.Model;
using gamespace.Model.Props;
using Microsoft.Xna.Framework;

namespace gamespaceunittests;

public class TileTest
{
    //This feels like overkill
    [Test]
    public void TestTileConstructor()
    {
        var prop = new Prop(Vector2.Zero, 1, 1, false);
        var tile = new Tile(prop);
        Assert.That(tile.Prop, Is.EqualTo(prop));
    }
}