using Microsoft.Xna.Framework;
using System;
using Tide.Core;

namespace Tide.Core
{
    public class FVectorHelper
    {
        public static Vector2 ToVector2(Vector3 vec)
        {
            return new Vector2(vec.X, vec.Y);
        }

        public static Vector3 ToVector3(Vector2 vec)
        {
            return new Vector3(vec.X, vec.Y, 0f);
        }
    }
}