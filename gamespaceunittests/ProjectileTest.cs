using gamespace.Model;
using gamespace.Model.Entities;
using Microsoft.Xna.Framework;

namespace gamespaceunittests;

public class ProjectileTest
{
    [Test]
    public void TestProjectileConstructorWithDirection()
    {
        var tempWorld = new World(1, 1);
        var tempPlayer = new Player("tester", tempWorld);
        var proj = new Projectile(1, 1, tempWorld, Vector2.Zero, Vector2.One, tempPlayer.EntityId);
        var expectedData = "Width: " + 1 + " Height: " + 1 + " Position: " + Vector2.Zero +
                           " MoveSpeed: " + new Vector2(1.5f, 1.5f) + " Sender: " + tempPlayer.EntityId;
        Assert.That(expectedData, Is.EqualTo(proj.ToString()));
    }

    [Test]
    public void TestProjectileConstructorWithoutDirection()
    {
        var tempWorld = new World(1, 1);
        var tempPlayer = new Player("tester", tempWorld);
        var proj = new Projectile(1, 1, tempWorld, Vector2.Zero, Vector2.Zero, tempPlayer.EntityId);
        var expectedData = "Width: " + 1 + " Height: " + 1 + " Position: " + Vector2.Zero +
                           " MoveSpeed: " + new Vector2(0.12f * 1.5f, 0.12f * 1.5f) + " Sender: " + tempPlayer.EntityId;
        Assert.That(expectedData, Is.EqualTo(proj.ToString()));
    }
}