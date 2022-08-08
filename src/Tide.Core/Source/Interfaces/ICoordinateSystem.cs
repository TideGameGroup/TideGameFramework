using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tide.Core
{
    public interface ICoordinateSystem
    {
        public Vector2 ConvertTo(Vector2 vect);
        public float ConvertAngleTo(float angle);
        public float ConvertScaleTo(float scale);

        public Vector2 ConvertFrom(Vector2 vect);
        public float ConvertAngleFrom(float angle);
        public float ConvertScaleFrom(float scale);
    }
}
