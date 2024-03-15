using gamespace.Model;
using Microsoft.Xna.Framework;

namespace gamespaceunittests;

public class SpikeTest
{
    
    [Test]
    public void SpikeConstructorTest()
    {
        var testSpike = new Spikes(Vector2.Zero, 1, 1, true);
        const string expectedData = "World Coordinate: {X:0 Y:0} Width: 1 Height: 1 Has movement: False " +
                                    "Has friction: False Has collision: True";
        Assert.That(testSpike.ToString(), Is.EqualTo(expectedData));
    }

    [Test]
    public void SpikeTestInteract()
    {
        var player = new Player("temp", null);
        var testSpike = new Spikes(Vector2.Zero, 1, 1, true);
        var originalHealth = player.Health;
        testSpike.InteractWithPlayer(player);
        Assert.That(player.Health, Is.EqualTo(originalHealth - 1));
    }
}