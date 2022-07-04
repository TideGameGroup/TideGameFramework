using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Tide.Core
{
    public interface IDrawableCanvasScript
    {
        public void DrawUI(UViewport view2D, SpriteBatch spriteBatch, GameTime gameTime);
    }
}
