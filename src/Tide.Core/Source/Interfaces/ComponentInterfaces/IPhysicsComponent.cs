using Microsoft.Xna.Framework;

namespace Tide.Core
{
    public interface IPhysicsComponent
    {
        public void PrePhysics(GameTime gameTime);
        public void CollisionUpdate(GameTime gameTime);
        public void PhysicsUpdate(GameTime gameTime);
        public void PostPhysics(GameTime gameTime);
    }
}
