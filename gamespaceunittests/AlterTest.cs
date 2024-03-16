﻿using gamespace.Managers;
using gamespace.Model.Entities;
using gamespace.Model.Props;
using Microsoft.Xna.Framework;

namespace gamespaceunittests;

public class AlterTest
{
    
    [Test]
    public void AlterConstructorTest()
    {
        var testAlter = new Altar(Vector2.Zero, 1, 1, true);
        const string expectedData = "World Coordinate: {X:0 Y:0} Width: 1 Height: 1 Has movement: False " +
                                    "Has friction: False Has collision: True";
        Assert.That(testAlter.ToString(), Is.EqualTo(expectedData));
    }
    

    [Test]
    public void TestWinTrue()
    {
        
        Assert.Throws(typeof(ArgumentException), TestWinTrueHelper);
    }
    
    [Test]
    public void TestWinFalse()
    {
        
        Assert.DoesNotThrow(TestWinFalseHelper);
    }

    private void TestWinTrueHelper()
    {
        var tempPlayer = new Player("temp", null);
        for (int i = 0; i < 5; i++)
        {
            tempPlayer.AddToInventory(Build.Items.Cog());
        }
        var alter = new Altar(Vector2.Zero, 1, 1, false);
        alter.Win += WinGameTester;
        alter.InteractWithPlayer(tempPlayer);
    }
    
    private void TestWinFalseHelper()
    {
        var tempPlayer = new Player("temp", null);
        var alter = new Altar(Vector2.Zero, 1, 1, false);
        alter.Win += WinGameTester;
        alter.InteractWithPlayer(tempPlayer);
    }
    

    private void WinGameTester()
    {
        throw new ArgumentException ("GameWon");
    }
}