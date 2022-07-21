using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Tide.Core
{
    public interface IDrawableComponent
    {
        public void Draw(FView view2D, SpriteBatch spriteBatch, GameTime gameTime);
    }
}
