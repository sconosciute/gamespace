using System;
using gamespace.View;
using Microsoft.Xna.Framework;

namespace gamespace.Managers;

public class AnimationManager
{
    private Animation _animation;

    private float _timer;
    
    public Vector2 Position { get; set; }

    public AnimationManager(Animation animation)
    {
        _animation = animation;
    }

    public void Play(Animation animation)
    {
        // TODO: implement handling
    }

    public void Stop(Animation animation)
    {
        _timer = 0f;

        _animation.CurrentFrame = 0;
    }
    
    public void Update(GameTime gameTime)
    { 
        _timer += (float)gameTime.ElapsedGameTime.Ticks / TimeSpan.TicksPerSecond;

        if (_timer > _animation.FrameSpeed)
        {
            _timer = 0f;
            _animation.CurrentFrame++;

            if (_animation.CurrentFrame >= _animation.FrameCount)
            {
                _animation.CurrentFrame = 0;
            }
        }
    }
}