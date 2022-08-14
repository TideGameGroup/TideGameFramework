using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Tide.Core
{
    public class FView : IViewComponent
    {
        public Vector2 position;
        public Matrix ProjectionInverse;
        public Rectangle viewport;

        public FView(Viewport viewport, float scale = 1440)
        {
            position = Vector2.Zero;
            Scale = scale;

            this.viewport = viewport.Bounds;
            BuildMatrices();
        }

        public FView(Rectangle viewport, float scale = 1440)
        {
            position = Vector2.Zero;
            Scale = scale;

            this.viewport = viewport;
            BuildMatrices();
        }

        public Matrix Projection { get; set; }
        public float Scale { get; set; }
        public Matrix ViewMatrix { get; set; }
        public Matrix ViewProjectionMatrix { get; set; }

        public void BuildMatrices()
        {
            BuildProjectionMatrix();
            BuildViewMatrix();
            BuildViewProjectionMatrix();
        }

        public void BuildProjectionMatrix()
        {
            if (viewport.Width > viewport.Height)
            {
                Projection = Matrix.CreateScale(
                    (float)viewport.Width / Scale,
                    (float)viewport.Width / Scale,
                    1f);
            }
            else
            {
                Projection = Matrix.CreateScale(
                    (float)viewport.Height / Scale,
                    (float)viewport.Height / Scale,
                    1f);
            }
            ProjectionInverse = Matrix.Invert(Projection);
        }

        public void BuildViewMatrix()
        {
            Vector3 _position = Vector3.Zero;

            _position.X = position.X - Scale / 2;
            _position.Y = position.Y - (Scale / (viewport.Width / viewport.Height)) / 2;

            ViewMatrix = Matrix.CreateTranslation(_position);
            ViewMatrix = Matrix.Invert(ViewMatrix);
        }

        public void BuildViewProjectionMatrix()
        {
            ViewProjectionMatrix = ViewMatrix * Projection;
        }

        public Vector2 ScreenToWorld(Vector2 screen)
        {
            var matrix = Matrix.Invert(ViewProjectionMatrix);
            Vector2 res = Vector2.Transform(screen, matrix);
            res.Y = -res.Y;
            return res;
        }

        public Vector2 WorldToScreen(Vector2 world, out bool InFrustum)
        {
            world.Y = -world.Y;
            InFrustum = true; // in frustum?
            //InFrustum = comp.Z >= 0 && comp.Z <= 1.0f; // in frustum?
            return Vector2.Transform(world, ViewProjectionMatrix);
        }

        public void Translate(Vector2 vector2)
        {
            position += vector2;
            BuildViewMatrix();
            BuildViewProjectionMatrix();
        }
    }
}