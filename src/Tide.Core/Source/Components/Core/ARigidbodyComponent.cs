using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Tide.Core
{
    public struct FRigidbody2DComponentContructorArgs
    {
        public AColliderComponent collider2DComponent;
        public ATransform transforms;
    }

    public class ARigidbodyComponent : UComponent, IPhysicsComponent
    {
        private readonly AColliderComponent collider2DComponent;

        private readonly ATransform transforms;
        private List<Vector2> impulses = new List<Vector2>();
        private List<Vector2> accelerations = new List<Vector2>();


        private List<float> masses = new List<float>();
        private List<Vector2> velocities = new List<Vector2>();

        public ARigidbodyComponent(FRigidbody2DComponentContructorArgs args)
        {
            TrySetDefault(args.transforms, out transforms);
            TrySetDefault(args.collider2DComponent, out collider2DComponent);

            collider2DComponent.rigidbody2DComponent = this;
            collider2DComponent.BindAction(HandleCollision);
        }

        public int Count => transforms.Count;

        private void HandleCollision(int i, Vector2 normal, AColliderComponent other, int j, GameTime gameTime, bool shouldCalculate)
        {
            if (shouldCalculate == false)
            {
                return;
            }

            Vector2 dir = normal;
            dir.Normalize();

            float restitution = 1f;
            float m1 = masses[i];
            float m2 = other.rigidbody2DComponent.masses[j];
            
            float v1 = Vector2.Dot(velocities[i], dir);
            float v2 = Vector2.Dot(other.rigidbody2DComponent.velocities[j], dir);

            if (m1 > 0)
            {
                if (m2 > 0)
                {
                    float newV1 = (m1 * v1 + m2 * v2 - m2 * (v1 - v2) * restitution) / (m1 + m2);
                    float newV2 = (m1 * v1 + m2 * v2 - m1 * (v2 - v1) * restitution) / (m1 + m2);

                    velocities[i] += dir * (newV1 - v1);
                    other.rigidbody2DComponent.velocities[j] += dir * (newV2 - v2);
                    other.rigidbody2DComponent.impulses[j] = normal / 2f;
                }
                else
                {
                    velocities[i] = Vector2.Reflect(velocities[i], -dir);
                }

                impulses[i] = normal / 2f;
            }
            else if (m2 > 0)
            {
                Vector2 a = other.rigidbody2DComponent.velocities[j];
                Vector2 b = Vector2.Reflect(a, -dir);
                other.rigidbody2DComponent.velocities[j] = b;
                other.rigidbody2DComponent.impulses[j] = normal / 2f;
            }
        }

        /*
        void Integrate(Vector2 pos, Vector2 vel, Vector2 acc, Vector2 force, float m)
        {
            Vector2 dP = sum(pos, multiply(sum(vel + (acc * 0.5f * timestep)) + timestep));
            Vector2 dA = multiply(sum(divide(force, m), acc), 0.5f);
            Vector2 dV = sum(vel, multiply(dA, timestep));
		    return { dP, dV, dA
            };
        }
        */

        public void Add(float mass = 1f)
        {
            masses.Add(mass);
            impulses.Add(Vector2.Zero);
            velocities.Add(Vector2.Zero);
        }

        public void ApplyAcceleration(int i, Vector2 accel)
        {
            velocities[i] += accel;
        }

        public void ApplyForce(int i, Vector2 force)
        {
            if (masses[i] > 0f)
            {
                velocities[i] += force / masses[i];
            }
        }

        #region IPhysicsComponent

        public void CollisionUpdate(GameTime gameTime)
        {
        }

        public void PhysicsUpdate(GameTime gameTime)
        {
            for (int i = 0; i < impulses.Count; i++)
            {
                Vector2 pos = transforms.positions[i];
                pos.X += impulses[i].X;
                pos.Y += impulses[i].Y;
                pos.X += velocities[i].X * (float)gameTime.ElapsedGameTime.TotalSeconds;
                pos.Y += velocities[i].Y * (float)gameTime.ElapsedGameTime.TotalSeconds;

                impulses[i] = Vector2.Zero;
                transforms.positions[i] = pos;
            }
        }

        public void PostPhysics(GameTime gameTime)
        {
        }

        public void PrePhysics(GameTime gameTime)
        {
        }

        #endregion IPhysicsComponent
    }
}