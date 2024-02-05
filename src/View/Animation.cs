using System;
using Microsoft.Xna.Framework;

namespace gamespace.View;

public class Animation
{
        private int _firstFrameOfAnimation;
        private int _lastFrameOfAnimation;
        
        private float _max;
    
        public int currentFrame { private set; get; }
        private float _count;
        public void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.Ticks / TimeSpan.TicksPerSecond; //(int)Math.Ceiling(gameTime.ElapsedGameTime.TotalSeconds))?
            _count += deltaTime;
            if (_count >= _max)
            {
                _count = 0;
                currentFrame++;
                if (currentFrame > _lastFrameOfAnimation)
                {
                    currentFrame = _firstFrameOfAnimation;
                }
            }
        }
}