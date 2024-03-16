using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace gamespace.View;

public class Animation
{
    /// <summary>
    /// The texture to animate.
    /// </summary>
    private readonly Texture2D _texture;
    
    /// <summary>
    /// The width of the frame to show.
    /// </summary>
    private readonly int _frameWidth;
    
    /// <summary>
    /// The height of the frame to show.
    /// </summary>
    private readonly int _frameHeight;
    
    /// <summary>
    /// Total amount of frames to play.
    /// </summary>
    private readonly int _totalFrames;
    
    /// <summary>
    /// How long the frames should play.
    /// </summary>
    private readonly float _frameTime;
    
    /// <summary>
    /// The current frame displayed.
    /// </summary>
    private int _currentFrame;
    
    /// <summary>
    /// The action being displayed.
    /// </summary>
    private AnimationAction _action;
    
    /// <summary>
    /// Last up call for a timer.
    /// </summary>
    private double _lastUpCall;

    /// <summary>
    /// Creates an animation object.
    /// </summary>
    /// <param name="texture">The texture of the sprite sheet.</param>
    /// <param name="framesX">Width of frame display.</param>
    /// <param name="framesY">Height of frame display.</param>
    /// <param name="totalFrames">The amount of frames to play in the animation.</param>
    /// <param name="frameTime">The time for the frame to play.</param>
    /// <param name="action">The action being performed.</param>
    public Animation(Texture2D texture, int framesX, int framesY, int totalFrames, float frameTime,
        AnimationAction action)
    {
        _texture = texture;
        _frameWidth = framesX;
        _frameHeight = framesY;
        _totalFrames = totalFrames;
        _frameTime = frameTime;
        _action = action;

        UpdateSourceRectangle();
    }

    /// <summary>
    /// Property for retrieving the correct source rectangle.
    /// </summary>
    public Rectangle SourceRectangle { get; private set; }

    /// <summary>
    /// Update the animation when the player is moving.
    /// </summary>
    /// <param name="gameTime">The time/ticks of the game.</param>
    /// <param name="action">The action being performed.</param>
    public void Update(GameTime gameTime, AnimationAction action)
    {
        var now = gameTime.TotalGameTime.TotalMilliseconds;

        if (_action != action)
        {
            _action = action;
            UpdateSourceRectangle();
        }

        if (!(_lastUpCall == 0 || now - _lastUpCall >= _frameTime)) return;
        _lastUpCall = now;
        _currentFrame = (_currentFrame + 1) % _totalFrames;
        UpdateSourceRectangle();
    }

    /// <summary>
    /// Updates the source rectangle to display the correct sprite.
    /// </summary>
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

        var row = _currentFrame / (_texture.Width / _frameWidth) + rowOffset;
        var col = _currentFrame % (_texture.Height / _frameHeight);

        SourceRectangle = new Rectangle(col * _frameWidth, row * _frameHeight, _frameWidth, _frameHeight);
    }
}

/// <summary>
/// Enums of animation actions.
/// </summary>
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