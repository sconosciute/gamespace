﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace gamespace.View;

public class Animation
{
        private readonly Texture2D _texture;
        private readonly int _frameWidth;
        private readonly int _frameHeight;
        private readonly int _totalFrames;
        private readonly float _frameTime;
        private int _currentFrame;
        private float _timer;
        private AnimationAction _action;
        
        public Animation(Texture2D texture, int framesX, int framesY, int totalFrames, float frameTime, AnimationAction action)
        {
                _texture = texture;
                _frameWidth = framesX;
                _frameHeight = framesY;
                _totalFrames = totalFrames;
                _frameTime = frameTime;
                _timer = 0f;
                _currentFrame = 0;
                _action = action;
                
                UpdateSourceRectangle();
        }

        public Rectangle SourceRectangle { get; private set; }
        
        public void Update(GameTime gameTime, AnimationAction action)
        {
                if (_action != action)
                {
                        _action = action;
                        UpdateSourceRectangle();
                }

                _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (!(_timer > _frameTime)) return;
                _timer = 0f;
                _currentFrame = (_currentFrame + 1) % _totalFrames;
                UpdateSourceRectangle();
        }

        private void UpdateSourceRectangle()
        {
                var rowOffset = _action switch
                {
                        AnimationAction.S => 0,
                        AnimationAction.Sw => 1,
                        AnimationAction.W => 2,
                        AnimationAction.Nw => 3,
                        AnimationAction.N => 4,
                        AnimationAction.Ne => 5,
                        AnimationAction.E => 6,
                        AnimationAction.Se => 7,
                        _ => 0
                };

                var row = (_currentFrame % (_texture.Height / _frameHeight)) + rowOffset;
                var col = _currentFrame / (_texture.Height / _frameHeight);

                SourceRectangle = new Rectangle(col * _frameWidth, row * _frameHeight, _frameWidth, _frameHeight);
        }
}

public enum AnimationAction
{
        N,
        Ne,
        E,
        Se,
        S,
        Sw,
        W,
        Nw,
        Use,
        Die
}