using Microsoft.Xna.Framework;

namespace Tide.Core
{
    public interface IDrawableComponent
    {
        public void Draw(UView3D view3D, GameTime gameTime);
    }
}
