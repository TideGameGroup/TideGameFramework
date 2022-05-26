using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Tide.Core
{
    public interface IDrawableComponent2D
    {
        public void Draw2D(UView3D view3D, SpriteBatch spriteBatch, GameTime gameTime);
    }
}
