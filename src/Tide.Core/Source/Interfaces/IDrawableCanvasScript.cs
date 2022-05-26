using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Tide.Core
{
    public interface IDrawableCanvasScript
    {
        public void DrawUI(UView3D view3D, SpriteBatch spriteBatch, GameTime gameTime);
    }
}
