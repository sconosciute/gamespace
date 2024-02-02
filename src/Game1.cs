﻿using System;
using gamespace.Model;
using gamespace.View;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace gamespace;

public class Game1 : Game
{
    //Use these constants and set _dynamicResolution to false in order to have a static width and height
    //  Looking at implementing a JSON file to contain these system settings
    private const int DefaultResolutionWidth = 1280;
    private const int DefaultResolutionHeight = 1024;
    
    public static int ScreenHeight;
    public static int ScreenWidth;
    
    private readonly GraphicsDeviceManager _graphics;
    private Boolean _isFullScreen = false;
    private Boolean _dynamicResolution = false;
    
    private SpriteBatch _spriteBatch;
    private Camera _camera; 
    private Player _player; 
    private Texture2D _playerModel;
    private Texture2D _testingBackGround;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        //https://industrian.net/tutorials/changing-display-resolution/ CHANGING to correct resolution on current setup
        if (_graphics == null)
        {
            _graphics.ApplyChanges();
        }
        //Creates a new GraphicsAdapter, allowing Game1 to get the resolution of the users monitor, allowing for dynamic resolutions.
        GraphicsAdapter adapter = new GraphicsAdapter();
        if (_dynamicResolution) {
            _graphics.PreferredBackBufferWidth = adapter.CurrentDisplayMode.Width; 
            _graphics.PreferredBackBufferHeight = adapter.CurrentDisplayMode.Height; 
        }
        else
        {
            _graphics.PreferredBackBufferWidth = DefaultResolutionWidth;
            _graphics.PreferredBackBufferHeight = DefaultResolutionHeight;
        }
        if (_isFullScreen)
        {
            _graphics.IsFullScreen = _isFullScreen;
        }

        _graphics.ApplyChanges(); //Applies correct resolution and full screens
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        ScreenHeight = _graphics.PreferredBackBufferHeight; //Gets the height of the screen
        ScreenWidth = _graphics.PreferredBackBufferWidth;   //Gets the width of the screen
        _player = new Player("Player32", "Rogue", 1, new RenderObject(_playerModel, null), 0, 0, 32, 32, true,
            true, 100, 100, 100, 100, 10);
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _testingBackGround = Content.Load<Texture2D>("checkeredBoardBackGround");
        _playerModel = Content.Load<Texture2D>("playerCircle32");
        _camera = new Camera();
        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here
        PlayerInput(gameTime);
        
        _player.Update(gameTime); 
        _camera.centerOn(_player); //Calls this after every update to keep player centered
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here
        _spriteBatch.Begin(transformMatrix: _camera.Transform);
        //Vector2 halfSize = new Vector2(0.5f, 0.5f); //Can use this cut the player size in half
        Vector2 playerSize = Vector2.One;
        _spriteBatch.Draw(_testingBackGround, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, Vector2.One,
            SpriteEffects.None, 1f);
        _spriteBatch.Draw(_playerModel, _player.ReturnPos(), null, Color.White, 0f, Vector2.Zero, playerSize,  SpriteEffects.None, 0f); //Layer 0f background, 1f foreground
        //Should render object replace _spriteBatch?
        _spriteBatch.End();
        base.Draw(gameTime);
    }

    private void PlayerInput(GameTime gameTime)
    {
        var kstate = Keyboard.GetState();

            if (kstate.IsKeyDown(Keys.W))
            {
                _player.Move(0, -(_player.MoveSpeed * (int)Math.Ceiling(gameTime.ElapsedGameTime.TotalSeconds)));
            }

            if (kstate.IsKeyDown(Keys.S))
            {
                _player.Move(0, (_player.MoveSpeed * (int)Math.Ceiling(gameTime.ElapsedGameTime.TotalSeconds)));
            }

            if (kstate.IsKeyDown(Keys.A))
            {
                _player.Move(-(_player.MoveSpeed * (int)Math.Ceiling(gameTime.ElapsedGameTime.TotalSeconds)), 0);
            }

            if (kstate.IsKeyDown(Keys.D))
            {
                _player.Move((_player.MoveSpeed * (int)Math.Ceiling(gameTime.ElapsedGameTime.TotalSeconds)), 0);
            }
    }
}
