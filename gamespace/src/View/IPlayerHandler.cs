using gamespace.Model;
using gamespace.Util;

namespace gamespace.View;

public interface IPlayerHandler
{
    public void HandlePlayerStateEvent(in EventHelper.PlayerState args);
}