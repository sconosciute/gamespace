using gamespace.Model.Props;
using Microsoft.Xna.Framework;

namespace gamespaceunittests;

public class PropTest
{
    [Test]
    public void PropConstructorTest()
    {
        var testProp = new Prop(Vector2.Zero, 1, 1, true);
        const string expectedData = "World Coordinate: {X:0 Y:0} Width: 1 Height: 1 Has movement: False " +
                                    "Has friction: False Has collision: True";
        Assert.That(testProp.ToString(), Is.EqualTo(expectedData));
    }
}