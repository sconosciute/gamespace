namespace gamespace.View;

public class Animation
{
    private string _name;
    private int _x;
    private int _y;
    private int _frames;

    public Animation(string name, int x, int y, int frames)
    {
        _name = name;
        _x = x;
        _y = y;
        _frames = frames;
    }
}