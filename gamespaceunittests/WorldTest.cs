using gamespace.Model;

namespace gamespaceunittests;

public class WorldTest
{
    [Test]
    public void WorldConstructorTest()
    {
        var testerWorld = new World(3, 3);
        var expectedData = "Min X: " + "-1" + " Min Y: " + "-1" + " Max X: " + "1" + " Max Y: " + "1";
        Assert.That(testerWorld.ToString(), Is.EqualTo(expectedData));
    }

    [Test]
    public void WorldConstructorZeroWidthTest()
    {
        Assert.Throws(typeof(ArithmeticException), WorldConstructorZeroWidthTestHelper);
    }

    private void WorldConstructorZeroWidthTestHelper()
    {
        var zeroWidthWorld = new World(0, 1);
    }
    
    [Test]
    public void WorldConstructorNegativeWidthTest()
    {
        Assert.Throws(typeof(ArithmeticException), WorldConstructorNegativeWidthTestHelper);
    }

    private void WorldConstructorNegativeWidthTestHelper()
    {
        var negativeWidthWorld = new World(-1, 1);
    }
    
    [Test]
    public void WorldConstructorZeroHeightTest()
    {
        Assert.Throws(typeof(ArithmeticException), WorldConstructorZeroHeightTestHelper);
    }

    private void WorldConstructorZeroHeightTestHelper()
    {
        var zeroHeightWorld = new World(1, 0);
    }
    
    [Test]
    public void WorldConstructorNegativeHeightTest()
    {
        Assert.Throws(typeof(ArithmeticException), WorldConstructorNegativeHeightTestHelper);
    }

    private void WorldConstructorNegativeHeightTestHelper()
    {
        var negativeHeightWorld = new World(1, -1);
    }
}