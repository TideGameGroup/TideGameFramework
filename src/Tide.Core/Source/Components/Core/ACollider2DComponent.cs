using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Tide.XMLSchema;

namespace Tide.Core
{
    //todo replace Iscript2 with root component
    using CollisionCallback = Action<int, Vector2, ACollider2DComponent, int, GameTime, bool>;


    public class ACollider2DComponent : UComponent, IPhysicsComponent, ISerialisableComponent
    {
        private readonly List<ECollider2DType> colliderTypes = new List<ECollider2DType>();
        private readonly List<uint> layers = new List<uint>();
        private readonly List<uint> masks = new List<uint>();
        private readonly List<ShapeUnion> shapes = new List<ShapeUnion>();
        private readonly ATransform2D transforms;
        private readonly List<CollisionCallback> collisionCallbacks = new List<CollisionCallback>();
        protected readonly List<bool> bEnabled = new List<bool>();
        public uint layer = 0;
        public uint mask = 0;
        public AKinematic2DComponent kinematic2DComponent;
        public ARigidbody2DComponent rigidbody2DComponent;

        public ACollider2DComponent(ATransform2D transforms)
        {
            this.transforms = transforms;
            AllowInternalCollisions = false;
        }

        public static float GridSize => 2.0f;
        public bool AllowInternalCollisions { get; set; }
        public int Count => shapes.Count;

        internal static bool AABBAABBCollision(FAABB a, Vector3 aP, FAABB b, Vector3 bP, out Vector2 normal)
        {
            bool bCollides = false;
            Vector2 _hitA = Vector2.Zero;

            float nx = bP.X - aP.X;
            float xExtent = a.xExtent + b.xExtent;
            float xOverlap = xExtent - Math.Abs(nx);

            if (xOverlap > 0)
            {
                float ny = bP.Z - aP.Z;
                float yExtent = a.yExtent + b.yExtent;
                float yOverlap = yExtent - Math.Abs(ny);

                if (yOverlap > 0)
                {
                    bCollides = true;

                    if (xOverlap < yOverlap) // favour x over y
                    {
                        _hitA.X = (nx < 0) ? xOverlap : -xOverlap;
                    }
                    else
                    {
                        _hitA.Y = (ny < 0) ? yOverlap : -yOverlap;
                    }
                }
            }

            normal = _hitA;
            return bCollides;
        }

        internal static bool AABBCircleCollision(FAABB a, Vector3 aP, FCircle b, Vector3 bP, out Vector2 normal)
        {
            normal = Vector2.Zero;
            Vector2 ap2 = new Vector2(aP.X, aP.Z);
            Vector2 bp2 = new Vector2(bP.X, bP.Z);

            Vector2 dist = bp2 - ap2;
            dist.X = Math.Clamp(dist.X, -a.xExtent, a.xExtent);
            dist.Y = Math.Clamp(dist.Y, -a.yExtent, a.yExtent);
            Vector2 closest = ap2 + dist;
            Vector2 diff = bp2 - closest;

            float len = diff.Length();
            bool collides = len < b.radius;

            if (collides)
            {
                float overlap = Math.Abs(b.radius - len);
                //float overlap = Math.Abs(len - b.radius);
                diff.Normalize();
                normal = diff * overlap;
            }
            return collides;
        }

        internal static bool AABBConeCollision(FAABB a, Vector3 aP, FCone cone, Vector3 bP, out Vector2 normal)
        {
            normal = Vector2.Zero;
            return false;
        }

        internal static bool AABBRectangleCollision(FAABB a, Vector3 aP, FAABB b, Vector3 bP, out Vector2 normal)
        {
            normal = Vector2.Zero;
            return false;
        }

        internal static bool CalculateCollisionsBetweenColliderComponents(ACollider2DComponent iCollider, ACollider2DComponent jCollider, GameTime gameTime)
        {
            for (int i = 0; i < iCollider.Count; i++) // maybe multithread
            {
                for (int j = 0; j < jCollider.Count; j++) // multithread
                {
                    // test layers can collides
                    bool bBothEnabled = iCollider.bEnabled[i] && jCollider.bEnabled[j];
                    bool IJCanCollide = (iCollider.layers[i] & jCollider.masks[j]) > 0;
                    bool JICanCollide = (jCollider.layers[j] & iCollider.masks[i]) > 0;

                    if (!bBothEnabled || !IJCanCollide && !JICanCollide)
                    {
                        continue;
                    }

                    if (StaticNarrowPhase(iCollider, i, jCollider, j, out Vector2 normal))
                    {
                        if (JICanCollide)
                        {
                            foreach (var callback in iCollider.collisionCallbacks)
                            {
                                callback.Invoke(i, -normal, jCollider, j, gameTime, true);
                            }
                        }

                        if (IJCanCollide)
                        {
                            foreach (var callback in jCollider.collisionCallbacks)
                            {
                                callback.Invoke(j, normal, iCollider, i, gameTime, false);
                            }
                        }
                    }
                }
            }
            return true;
        }

        internal static bool CircleCircleCollision(FCircle a, Vector3 aP, FCircle b, Vector3 bP, out Vector2 normal)
        {
            float fRadiiSum = a.radius + b.radius;
            Vector2 dir = FVectorHelper.ToVector2(aP - bP);
            float fDist = MathF.Sqrt(Vector2.Dot(dir, dir));

            normal = -Vector2.Normalize(dir) * (fDist - fRadiiSum);

            return fDist < fRadiiSum;
        }

        internal static bool ConeConeCollision(FCone a, Vector3 aP, float aA, FCone b, Vector3 bP, float bA)
        {
            return false;
        }

        internal static bool ConeRectangleCollision(FCone c, Vector3 cP, float cA, FRectangle a, Vector3 aP, float aA)
        {
            return false;
        }

        //https://bartwronski.com/2017/04/13/cull-that-cone/
        internal static bool ConeSphereCollision(FCone c, Vector3 cP, float cA, FCircle b, Vector3 bP)
        {
            float coneForward = cA;
            float coneAngle = c.angle;

            Vector3 vecD = bP - cP;
            float fDistSqr = Vector3.Dot(vecD, vecD);
            float V1len = Vector3.Dot(vecD, new Vector3(MathF.Sin(coneForward), MathF.Cos(coneForward), 0.0f));
            float distanceClosestPoint = MathF.Cos(coneAngle) * MathF.Sqrt(fDistSqr - V1len * V1len) - V1len * MathF.Sin(coneAngle);

            float rad = b.radius;
            bool angleCull = distanceClosestPoint > rad;
            bool frontCull = V1len > rad + c.length;
            bool backCull = V1len < -rad;

            return !(angleCull || frontCull || backCull);
        }

        internal static GridCell GetCell(Vector3 position)
        {
            GridCell newCell;
            newCell.X = (int)Math.Floor(position.X / GridSize);
            newCell.Y = (int)Math.Floor(position.Z / GridSize);
            return newCell;
        }

        internal static bool RectangleRectangleCollision(FRectangle a, Vector3 aP, float aA, FRectangle b, Vector3 bP, float bA)
        {
            return false;
        }

        internal static bool RectangleSphereCollision(FRectangle a, Vector3 aP, float aA, FCircle b, Vector3 bP)
        {
            return false;
        }

        internal static bool StaticBroadPhase(ACollider2DComponent iCollider, int i, ACollider2DComponent jCollider, int j)
        {
            GridCell a = GetCell(iCollider.transforms.positions[i]);
            GridCell b = GetCell(jCollider.transforms.positions[j]);

            int cellDeltaX = Math.Abs(a.X - b.X);
            int cellDeltaY = Math.Abs(a.Y - b.Y);

            return cellDeltaX < 2 && cellDeltaY < 2;
        }

        internal static bool StaticNarrowPhase(ACollider2DComponent iCollider, int i, ACollider2DComponent jCollider, int j, out Vector2 normal)
        {
            bool collision = false;

            normal = Vector2.Zero;

            // collider i
            switch (iCollider.colliderTypes[i])
            {
                case ECollider2DType.EAABB:

                    // collider j
                    switch (jCollider.colliderTypes[j])
                    {
                        case ECollider2DType.EAABB:
                            collision = AABBAABBCollision(
                                iCollider.shapes[i].aabb,
                                iCollider.transforms.positions[i],
                                jCollider.shapes[j].aabb,
                                jCollider.transforms.positions[j],
                                out normal);
                            break;

                        case ECollider2DType.ECIRCLE:
                            collision = AABBCircleCollision(
                                iCollider.shapes[i].aabb,
                                iCollider.transforms.positions[i],
                                jCollider.shapes[j].circle,
                                jCollider.transforms.positions[j],
                                out normal);
                            normal = -normal;
                            break;

                        case ECollider2DType.ECONE:
                            break;

                        case ECollider2DType.ERECTANGLE:
                            break;
                    }
                    break;

                case ECollider2DType.ECIRCLE:

                    // collider j
                    switch (jCollider.colliderTypes[j])
                    {
                        case ECollider2DType.EAABB:
                            collision = AABBCircleCollision(
                                jCollider.shapes[j].aabb,
                                jCollider.transforms.positions[j],
                                iCollider.shapes[i].circle,
                                iCollider.transforms.positions[i],
                                out normal);
                            break;

                        case ECollider2DType.ECIRCLE:
                            collision = CircleCircleCollision(
                                iCollider.shapes[i].circle,
                                iCollider.transforms.positions[i],
                                jCollider.shapes[j].circle,
                                jCollider.transforms.positions[j],
                                out normal);
                            break;

                        case ECollider2DType.ECONE:
                            break;

                        case ECollider2DType.ERECTANGLE:
                            break;
                    }
                    break;

                case ECollider2DType.ECONE:

                    // collider j
                    switch (jCollider.colliderTypes[j])
                    {
                        case ECollider2DType.EAABB:
                            break;

                        case ECollider2DType.ECIRCLE:
                            break;

                        case ECollider2DType.ECONE:
                            break;

                        case ECollider2DType.ERECTANGLE:
                            break;
                    }
                    break;

                case ECollider2DType.ERECTANGLE:
                    // collider j
                    switch (jCollider.colliderTypes[j])
                    {
                        case ECollider2DType.EAABB:
                            break;

                        case ECollider2DType.ECIRCLE:
                            break;

                        case ECollider2DType.ECONE:
                            break;

                        case ECollider2DType.ERECTANGLE:
                            break;
                    }
                    break;
            }

            return collision;
        }

        public static void StaticTickImplementation(UComponentGraph ComponentGraph, GameTime gameTime)
        {
            List<ACollider2DComponent> StaticColliders = ComponentGraph.FindAll<ACollider2DComponent>();

            // iterate over colliders and calculate collisions
            for (int i = 0; i < StaticColliders.Count; i++)
            {
                for (int j = i + 1; j < StaticColliders.Count; j++)
                {
                    // skip if internal collisions are disabled
                    if (i == j && !StaticColliders[i].AllowInternalCollisions)
                    {
                        continue;
                    }

                    // do a broad bit comparison to see if any objects are even on layers that can collide
                    if ((StaticColliders[i].layer & StaticColliders[j].mask) > 0 ||
                        (StaticColliders[j].layer & StaticColliders[i].mask) > 0)
                    {
                        CalculateCollisionsBetweenColliderComponents(StaticColliders[i], StaticColliders[j], gameTime);
                    }
                }
            }
        }

        public void Add(ShapeUnion shape, ECollider2DType colliderType, uint _layer = 0, uint _mask = 0)
        {
            shapes.Add(shape);
            layers.Add(_layer == 0 ? layer : _layer);
            masks.Add(_mask == 0 ? mask : _mask);
            colliderTypes.Add(colliderType);
            bEnabled.Add(true);

            if (_layer >= 0)
            {
                layer |= _layer;
            }
            if (_layer >= 0)
            {
                mask |= _mask;
            }
        }

        public void Add(FCone shape, uint _layer = 0, uint _mask = 0)
        {
            ShapeUnion newShape = new ShapeUnion
            {
                cone = shape
            };

            Add(newShape, ECollider2DType.ECONE, _layer, _mask);
        }

        public void Add(FCircle shape, uint _layer = 0, uint _mask = 0)
        {
            ShapeUnion newShape = new ShapeUnion
            {
                circle = shape
            };

            Add(newShape, ECollider2DType.ECIRCLE, _layer, _mask);
        }

        public void Add(FAABB shape, uint _layer = 0, uint _mask = 0)
        {
            ShapeUnion newShape = new ShapeUnion
            {
                aabb = shape
            };

            Add(newShape, ECollider2DType.EAABB, _layer, _mask);
        }

        public void Add(FRectangle shape, uint _layer = 0, uint _mask = 0)
        {
            ShapeUnion newShape = new ShapeUnion
            {
                rectangle = shape
            };

            Add(newShape, ECollider2DType.ERECTANGLE, _layer, _mask);
        }

        public int BindAction(CollisionCallback callback)
        {
            collisionCallbacks.Add(callback);
            return collisionCallbacks.Count - 1;
        }

        public void RemoveAt(int i)
        {
            shapes.RemoveAt(i);
            colliderTypes.RemoveAt(i);
            layers.RemoveAt(i);
            masks.RemoveAt(i);
            bEnabled.RemoveAt(i);
        }

        public void SetEnabled(int i, bool val)
        {
            bEnabled[i] = val;
        }

        public void SetLayer(int bit, bool val)
        {
            for (int i = 0; i < layers.Count; i++)
            {
                layers[i] = FBitHelper.SetBit(bit, val, layers[i]);
            }
            FBitHelper.SetBit(bit, val, ref layer);
        }

        public void SetMask(int bit, bool val)
        {
            for (int i = 0; i < masks.Count; i++)
            {
                layers[i] = FBitHelper.SetBit(bit, val, layers[i]);
            }
            FBitHelper.SetBit(bit, val, ref mask);
        }

        #region IPhysicsComponent

        public void CollisionUpdate(GameTime gameTime)
        {
            if (this == ScriptGraph.Find<ACollider2DComponent>())
            {
                StaticTickImplementation(ScriptGraph, gameTime);
            }
        }

        public void PhysicsUpdate(GameTime gameTime)
        {
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
                FCollider2D loaded = content.Load<FCollider2D>(serialisedDataPath);

                for (int i = 0; i < loaded.colliderTypes.Length; i++)
                {
                    Add(loaded.shapes[i], loaded.colliderTypes[i], loaded.layers[i], loaded.masks[i]);
                }
            }
        }

        public string Serialise(string path, ref Dictionary<string, ISerialisedInstanceData> serialisedSet)
        {
            string ID = ISerialisableComponent.GetSerialisableID(this, path, ref serialisedSet);

            serialisedSet.Add(ID, new FCollider2D
            {
                colliderTypes = colliderTypes.ToArray(),
                layers = layers.ToArray(),
                masks = masks.ToArray(),
                shapes = shapes.ToArray()
            }
            );
            return ID;
        }

    #endregion ISerialisableComponent
}
}