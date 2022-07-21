using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tide.Core
{
    public interface ISystem
    {
        public void Draw(TComponentGraph graph, GameTime gameTime);
        public void Update(TComponentGraph graph, GameTime gameTime);
    }
}
