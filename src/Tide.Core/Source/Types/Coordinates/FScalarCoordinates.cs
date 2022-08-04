using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Tide.Core
{
    public class FScalarCoordinates : ICoordinateSystem
    {
        private readonly float scale = 1f;

        public FScalarCoordinates(float scale)
        {
            this.scale = scale;
        }

        public float ConvertAngleFrom(float angle)
        {
            return angle;
        }

        public float ConvertAngleTo(float angle)
        {
            return angle;
        }

        public Vector2 ConvertFrom(Vector2 vect)
        {
            return vect / scale;
        }

        public float ConvertScaleFrom(float scale)
        {
            return scale;
        }

        public float ConvertScaleTo(float scale)
        {
            return scale;
        }

        public Vector2 ConvertTo(Vector2 vect)
        {
            return vect * scale;
        }
    }
}