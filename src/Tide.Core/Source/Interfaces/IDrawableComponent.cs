﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Tide.Core
{
    public interface IDrawableComponent
    {
        public void Draw2D(UViewport view2D, SpriteBatch spriteBatch, GameTime gameTime);
    }
}
