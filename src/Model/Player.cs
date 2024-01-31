﻿using System;
using System.Diagnostics;
using gamespace.View;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace gamespace.Model;

//Side note, we should probably follow a singleton principle with player either here or in main right?
// like this? public static Player Instance = new Player(); ~ Logan
public class Player : Character
{
    
    private string _name;   //stores the players name
    private string _playerClass;  //stores the players class;
    
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
    
    //Not sure how to handle this, without a set method or having x/y be protected. 
    // unless we want to make this not abstract earlier and define it in entity? Temporally added setters in physics obj
    public override void Move(int x, int y)
    {
        SetX(GetX() + x);
        SetY(GetY() + y);
    }

    public Vector2 ReturnPos()
    {
        return new Vector2(GetX(), GetY());
    }
    //Not sure what to do here?
    //Needed to add gameTime param in order to track how long user held key down.
    public override void Update(GameTime gameTime)
    {
        var kstate = Keyboard.GetState();

        if (kstate.IsKeyDown(Keys.W))
        {
            this.Move(0, -(this.getMoveSpeed() * (int)Math.Ceiling(gameTime.ElapsedGameTime.TotalSeconds))); 
        }

        if (kstate.IsKeyDown(Keys.S))
        {
            this.Move(0, (this.getMoveSpeed() * (int)Math.Ceiling(gameTime.ElapsedGameTime.TotalSeconds)));
        }

        if (kstate.IsKeyDown(Keys.A))
        {
            this.Move(-(this.getMoveSpeed() * (int)Math.Ceiling(gameTime.ElapsedGameTime.TotalSeconds)), 0);
        }

        if (kstate.IsKeyDown(Keys.D))
        {
            this.Move((this.getMoveSpeed() * (int)Math.Ceiling(gameTime.ElapsedGameTime.TotalSeconds)), 0);
        }
        //throw new System.NotImplementedException();
    }
}