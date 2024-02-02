using System;
using System.Diagnostics;
using gamespace.View;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace gamespace.Model;

//Side note, we should probably follow a singleton principle with player either here or in main right?
// like this? public static Player Instance = new Player(); ~ Logan
public class Player : Character {
    
    private string _name;   
    private string _playerClass;  
    
    public Player(string name, string playerClass, int moveSpeed, RenderObject sprite, int x, int y, int width, int height, bool canCollide,
        bool canMove, int hp, int maxHp, int energy, int maxEnergy, int baseDmg) : base(moveSpeed, sprite,  x, y,  width,  height,  canCollide,
         canMove, hp, maxHp, energy, maxEnergy, baseDmg)
    {
        _name = name;
        _playerClass = playerClass;
    }

    //inventory
    //special ability
    
    //inventory functions
    
    //special ability function;
    
    public override void Move(int newX, int newY)
    {
        X += newX;
        Y += newY;
    }

    public Vector2 ReturnPos()
    {
        return new Vector2(X, Y);
    }
    //No longer used here, to avoid breaking MVC conventions. At least not for movement, may remove gameTime
    public override void Update(GameTime gameTime)
    {
        //throw new System.NotImplementedException()
    }
}