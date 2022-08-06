using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Tide.Core
{
    public class FIsometricCoordinates : ICoordinateSystem
    {
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
            return FStaticIsometricFunctions.ToCartesian(vect);
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
            return FStaticIsometricFunctions.ToIso(vect);
        }
    }
}