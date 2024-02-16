using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace gamespace.View;

public class Animation
{
        private int _firstFrameOfAnimation;
        private int _lastFrameOfAnimation;
        
        private float _max;
    
        public int currentFrame { private set; get; }
        private float _count;
        
        public int CurrentFrame { get; set; }

        public int FrameCount { get; private set; }

        public int FrameHeight { get { return Texture.Height; } }

        public float FrameSpeed { get; set; }

        public int FrameWidth { get { return Texture.Width / FrameCount; } }

        public bool IsLooping { get; set; }

        public Texture2D Texture { get; private set; }

        public Animation(Texture2D texture, int frameCount)
        {
                Texture = texture;

                FrameCount = frameCount;

                IsLooping = true;

                FrameSpeed = 0.2f;
        }
        
        //TODO: review
        // public void Update(GameTime gameTime)
        // {
        //     float deltaTime = (float)gameTime.ElapsedGameTime.Ticks / TimeSpan.TicksPerSecond; //(int)Math.Ceiling(gameTime.ElapsedGameTime.TotalSeconds))?
        //     _count += deltaTime;
        //     if (_count >= _max)
        //     {
        //         _count = 0;
        //         currentFrame++;
        //         if (currentFrame > _lastFrameOfAnimation)
        //         {
        //             currentFrame = _firstFrameOfAnimation;
        //         }
        //     }
        // }
}