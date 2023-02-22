using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Tide.XMLSchema;

namespace Tide.Core
{
    public struct FKinematic2DComponentContructorArgs
    {
        public AColliderComponent collider2DComponent;
        public ATransformComponent transforms;
    }

    public class AKinematicComponent : UComponent, IPhysicsComponent, ISerialisableComponent
    {
        private readonly AColliderComponent collider2DComponent;
        private readonly ATransformComponent transforms;

        private List<bool> bIsFixed = new List<bool>();
        private List<Vector2> impulses = new List<Vector2>();

        public AKinematicComponent(FKinematic2DComponentContructorArgs args)
        {
            TrySetDefault(args.transforms, out transforms);
            TrySetDefault(args.collider2DComponent, out collider2DComponent);

            collider2DComponent.kinematic2DComponent = this;
            collider2DComponent.BindAction(HandleCollision);
        }

        public int Count => transforms.Count;

        private void HandleCollision(int i, Vector2 normal, AColliderComponent other, int j, GameTime gameTime, bool shouldCalculate)
        {
            if (bIsFixed[i] == false)
            {
                if (other.kinematic2DComponent == null || other.kinematic2DComponent.bIsFixed[j])
                {
                    impulses[i] += -normal;
                }
                else
                {
                    impulses[i] += -normal / 2f;
                }
            }
        }

        public void Add(bool isFixed = false)
        {
            bIsFixed.Add(isFixed);
            impulses.Add(Vector2.Zero);
        }

        public void RemoveAt(int i)
        {
            bIsFixed.RemoveAt(i);
            impulses.RemoveAt(i);
        }

        public void ApplyImpulse(int i, Vector2 impulse)
        {
            if (!bIsFixed[i])
            {
                impulses[i] += impulse;
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
                Vector2 pos = transforms.worldPositions[i];
                pos.X += impulses[i].X;
                pos.Y += impulses[i].Y;
                impulses[i] = Vector2.Zero;
                transforms.SetPosition(i, pos);
            }
        }

        public void PostPhysics(GameTime gameTime)
        {
        }

        public void PrePhysics(GameTime gameTime)
        {
        }
        #endregion IPhysicsComponent

        #region ISerialisableComponent

        public void Load(UContentManager content, string serialisedDataPath)
        {
            if (serialisedDataPath != null && serialisedDataPath != "")
            {
                FKinematics loaded = content.Load<FKinematics>(serialisedDataPath);

                bIsFixed = new List<bool>(loaded.bIsFixed);
                for(int i = 0; i < bIsFixed.Count; i++ )
                {
                    impulses.Add(Vector2.Zero);
                }
            }
        }

        public string Serialise(string path, ref Dictionary<string, ISerialisedInstanceData> serialisedSet)
        {
            string ID = ISerialisableComponent.GetSerialisableID(this, path, ref serialisedSet);

            serialisedSet.Add(ID, new FKinematics
            {
                bIsFixed = bIsFixed.ToArray()
            }
            );
            return ID;
        }

        #endregion
    }
}