using System;
using gamespace.Model;
using gamespace.View;
using Microsoft.Xna.Framework;

namespace gamespace.Managers;

public static class Build
{
    public struct Props
    {
        public static Prop Floor(GameManager gm, Vector2 worldPosition, out RenderObject renderable)
        {
            var prop = new Prop(worldPosition, 1, 1, false);
            renderable = new RenderObject(
                texture: gm.GetTexture(Textures.TestTile),
                worldPosition: worldPosition,
                layerDepth: LayerDepth.Background);
            return prop;
        }

        public static Prop Wall(GameManager gm, Vector2 worldPosition, out RenderObject renderable)
        {
            var prop = new Prop(worldPosition, 1, 1, true);
            renderable = new RenderObject(
                texture: gm.GetTexture(Textures.Collider),
                worldPosition: worldPosition,
                layerDepth: LayerDepth.Background);
            return prop;
        }
    }
}
