﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Tide.Core
{
    //todo replace Iscript2 with root component
    using CollisionCallback = Action<int, ACollider2DComponent, int, GameTime>;

    public enum ECollider2DType
    {
        ECIRCLE,
        ECONE,
        ERECTANGLE
    }

    public struct Circle
    {
        public float radius;

        public Circle(float radius)
        {
            this.radius = radius;
        }

        public static Circle Unit => new Circle(1.0f);
    }

    public struct Cone
    {
        public float angle;
        public float direction;
        public float length;
        // vector?

        //new Vector3(MathF.Sin(coneForward), MathF.Cos(coneForward), 0.0f)
    }

    public struct GridCell
    {
        public int X;
        public int Y;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct ShapeUnion
    {
        [FieldOffset(0)]
        public Circle circle;

        [FieldOffset(0)]
        public Cone cone;

        [FieldOffset(0)]
        public Rectangle rectangle;
    }

    public class ACollider2DComponent : UComponent, IUpdateComponent
    {
        private static List<ACollider2DComponent> StaticColliders = new List<ACollider2DComponent>();

        private readonly ATransform2D transforms;

        // collision callbacks
        // each is called on collisions returning the colliding components and indices for the entity
        private List<CollisionCallback> collisionCallbacks = new List<CollisionCallback>();

        // component data
        //public readonly List<UInt16> layers = new List<UInt16>();

        //public readonly List<UInt16> masks = new List<UInt16>();
        public readonly List<ShapeUnion> shapes = new List<ShapeUnion>(); //in radians

        public List<ECollider2DType> colliderTypes = new List<ECollider2DType>(); //in radians

        public ACollider2DComponent(ATransform2D transforms)
        {
            StaticColliders.Add(this);

            this.transforms = transforms;

            AllowInternalCollisions = true;
        }

        public static float GridSize => 100.0f;
        public bool AllowInternalCollisions { get; set; }
        public int Count => shapes.Count;

        internal static bool CalculateCollisionsBetweenColliderComponents(ACollider2DComponent iCollider, ACollider2DComponent jCollider, GameTime gameTime)
        {
            for (int i = 0; i < iCollider.Count; i++) // maybe multithread
            {
                for (int j = i + 1; j < jCollider.Count; j++) // multithread
                {
                    // calculate whether these 2 shapes collide
                    if (StaticBroadPhase(iCollider, i, jCollider, j) &&
                        StaticNarrowPhase(iCollider, i, jCollider, j))
                    {
                        // callbacks
                        foreach (var callback in iCollider.collisionCallbacks)
                        {
                            callback.Invoke(i, jCollider, j, gameTime);
                        }

                        foreach (var callback in jCollider.collisionCallbacks)
                        {
                            callback.Invoke(j, iCollider, i, gameTime);
                        }
                    }
                }
            }
            return true;
        }

        internal static bool ConeConeCollision(Cone a, Vector3 aP, float aA, Cone b, Vector3 bP, float bA)
        {
            return false;
        }

        internal static bool ConeRectangleCollision(Cone c, Vector3 cP, float cA, Rectangle a, Vector3 aP, float aA)
        {
            return false;
        }

        //https://bartwronski.com/2017/04/13/cull-that-cone/
        internal static bool ConeSphereCollision(Cone c, Vector3 cP, float cA, Circle b, Vector3 bP)
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

        internal static bool RectangleRectangleCollision(Rectangle a, Vector3 aP, float aA, Rectangle b, Vector3 bP, float bA)
        {
            return false;
        }

        internal static bool RectangleSphereCollision(Rectangle a, Vector3 aP, float aA, Circle b, Vector3 bP)
        {
            return false;
        }

        internal static bool SphereSphereCollision(Circle a, Vector3 aP, Circle b, Vector3 bP)
        {
            float fRadiiSumSquared = a.radius + b.radius;
            fRadiiSumSquared *= fRadiiSumSquared;

            Vector3 vecDist = aP - bP;
            float fDistSqr = Vector3.Dot(vecDist, vecDist);

            return fDistSqr < fRadiiSumSquared;
        }

        internal static bool StaticBroadPhase(ACollider2DComponent iCollider, int i, ACollider2DComponent jCollider, int j)
        {
            GridCell a = GetCell(iCollider.transforms.positions[i]);
            GridCell b = GetCell(jCollider.transforms.positions[j]);

            int cellDeltaX = Math.Abs(a.X - b.X);
            int cellDeltaY = Math.Abs(a.Y - b.Y);

            return cellDeltaX < 2 && cellDeltaY < 2;
        }

        internal static bool StaticNarrowPhase(ACollider2DComponent iCollider, int i, ACollider2DComponent jCollider, int j)
        {
            bool collision = false;

            // collider i
            switch (iCollider.colliderTypes[i])
            {
                case ECollider2DType.ECIRCLE:

                    // collider j
                    switch (jCollider.colliderTypes[j])
                    {
                        case ECollider2DType.ECIRCLE:
                            collision = SphereSphereCollision(
                                iCollider.shapes[i].circle,
                                iCollider.transforms.positions[i],
                                jCollider.shapes[j].circle,
                                jCollider.transforms.positions[j]);
                            break;

                        case ECollider2DType.ECONE:
                            collision = ConeSphereCollision(
                                jCollider.shapes[j].cone,
                                jCollider.transforms.positions[j],
                                jCollider.transforms.angles[j],

                                iCollider.shapes[i].circle,
                                iCollider.transforms.positions[i]);
                            break;

                        case ECollider2DType.ERECTANGLE:
                            collision = RectangleSphereCollision(
                                jCollider.shapes[j].rectangle,
                                jCollider.transforms.positions[j],
                                jCollider.transforms.angles[j],

                                iCollider.shapes[i].circle,
                                iCollider.transforms.positions[i]);
                            break;
                    }
                    break;

                case ECollider2DType.ECONE:

                    // collider j
                    switch (jCollider.colliderTypes[j])
                    {
                        case ECollider2DType.ECIRCLE:
                            collision = ConeSphereCollision(
                                iCollider.shapes[i].cone,
                                iCollider.transforms.positions[i],
                                iCollider.transforms.angles[i],

                                jCollider.shapes[j].circle,
                                jCollider.transforms.positions[j]);
                            break;

                        case ECollider2DType.ECONE:
                            collision = ConeConeCollision(
                                jCollider.shapes[j].cone,
                                jCollider.transforms.positions[j],
                                jCollider.transforms.angles[j],

                                iCollider.shapes[i].cone,
                                iCollider.transforms.positions[i],
                                iCollider.transforms.angles[i]);
                            break;

                        case ECollider2DType.ERECTANGLE:
                            collision = ConeRectangleCollision(
                                iCollider.shapes[i].cone,
                                iCollider.transforms.positions[i],
                                iCollider.transforms.angles[i],

                                jCollider.shapes[j].rectangle,
                                jCollider.transforms.positions[j],
                                jCollider.transforms.angles[j]);
                            break;
                    }
                    break;

                case ECollider2DType.ERECTANGLE:
                    // collider j
                    switch (jCollider.colliderTypes[j])
                    {
                        case ECollider2DType.ECIRCLE:
                            collision = RectangleSphereCollision(
                                iCollider.shapes[i].rectangle,
                                iCollider.transforms.positions[i],
                                iCollider.transforms.angles[i],

                                jCollider.shapes[j].circle,
                                jCollider.transforms.positions[j]);
                            break;

                        case ECollider2DType.ECONE:
                            collision = ConeRectangleCollision(
                                jCollider.shapes[j].cone,
                                jCollider.transforms.positions[j],
                                jCollider.transforms.angles[j],

                                iCollider.shapes[i].rectangle,
                                iCollider.transforms.positions[i],
                                iCollider.transforms.angles[i]);
                            break;

                        case ECollider2DType.ERECTANGLE:
                            collision = RectangleRectangleCollision(
                                iCollider.shapes[i].rectangle,
                                iCollider.transforms.positions[i],
                                iCollider.transforms.angles[i],

                                jCollider.shapes[j].rectangle,
                                jCollider.transforms.positions[j],
                                jCollider.transforms.angles[j]);
                            break;
                    }
                    break;
            }

            return collision;
        }

        internal bool CheckCollision(Rectangle rectangle, int i, ref Rectangle result)
        {
            result = Rectangle.Intersect(rectangle, shapes[i].rectangle);
            return !result.IsEmpty;
        }

        // static implementation
        public static void StaticTickImplementation(GameTime gameTime)
        {
            // iterate over colliders and calculate collisions
            for (int i = 0; i < StaticColliders.Count; i++)
            {
                for (int j = i; j < StaticColliders.Count; j++)
                {
                    // skip if internal collisions are disabled
                    if (i == j && !StaticColliders[i].AllowInternalCollisions)
                    {
                        continue;
                    }

                    CalculateCollisionsBetweenColliderComponents(StaticColliders[i], StaticColliders[j], gameTime);
                }
            }
        }

        public void Add(ShapeUnion shape)
        {
            shapes.Add(shape);
            colliderTypes.Add(ECollider2DType.ECIRCLE);
        }

        public void Add(Cone shape)
        {
            ShapeUnion newShape = new ShapeUnion
            {
                cone = shape
            };

            shapes.Add(newShape);
            colliderTypes.Add(ECollider2DType.ECONE);
        }

        public void Add(Circle shape)
        {
            ShapeUnion newShape = new ShapeUnion
            {
                circle = shape
            };

            shapes.Add(newShape);
            colliderTypes.Add(ECollider2DType.ECIRCLE);
        }

        public void Add(Rectangle shape)
        {
            ShapeUnion newShape = new ShapeUnion
            {
                rectangle = shape
            };

            shapes.Add(newShape);
            colliderTypes.Add(ECollider2DType.ERECTANGLE);
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
        }

        public void Update(GameTime gameTime)
        {
            // only 1 collider should update,
            // this calls a static function that calls all 2d collision components together
            if (this == StaticColliders[0])
            {
                StaticTickImplementation(gameTime);
            }
        }
    }
}