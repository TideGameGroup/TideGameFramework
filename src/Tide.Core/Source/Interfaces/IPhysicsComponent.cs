using Microsoft.Xna.Framework;

namespace Tide.Core
{
    public interface IPhysicsComponent
    {
        public void PrePhysics(float step);
        public void CollisionUpdate(float step);
        public void PhysicsUpdate(float step);
        public void PostPhysics(float step);
    }
}
