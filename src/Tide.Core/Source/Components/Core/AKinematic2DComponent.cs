using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Tide.XMLSchema;

namespace Tide.Core
{
    public struct FKinematic2DComponentContructorArgs
    {
        public ACollider2DComponent collider2DComponent;
        public ATransform2D transforms;
    }

    public class AKinematic2DComponent : UComponent, IPhysicsComponent, ISerialisableComponent
    {
        private readonly ACollider2DComponent collider2DComponent;
        private readonly ATransform2D transforms;

        private List<bool> bIsFixed = new List<bool>();
        private List<Vector2> impulses = new List<Vector2>();

        public AKinematic2DComponent(FKinematic2DComponentContructorArgs args)
        {
            TrySetDefault(args.transforms, out transforms);
            TrySetDefault(args.collider2DComponent, out collider2DComponent);

            collider2DComponent.kinematic2DComponent = this;
            collider2DComponent.BindAction(HandleCollision);
        }

        public int Count => transforms.Count;

        private void HandleCollision(int i, Vector2 normal, ACollider2DComponent other, int j, float step, bool shouldCalculate)
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

        public void CollisionUpdate(float step)
        {
        }

        public void PhysicsUpdate(float step)
        {
            for (int i = 0; i < impulses.Count; i++)
            {
                Vector3 pos = transforms.positions[i];
                pos.X += impulses[i].X;
                pos.Z += impulses[i].Y;
                impulses[i] = Vector2.Zero;
                transforms.positions[i] = pos;
            }
        }

        public void PostPhysics(float step)
        {
        }

        public void PrePhysics(float step)
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