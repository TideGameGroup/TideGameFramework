using Microsoft.Xna.Framework;
using System;
using Tide.Core;

namespace Tide.Core
{
    public class FStaticIsometricFunctions
    {
        public static Vector2 ToIso(Vector2 coord)
        {
            return new Vector2(
                coord.X - coord.Y,
                (coord.X + coord.Y) / 2
            );
        }

        public static Vector2 ToIso(Vector2 coord, float scale)
        {
            coord *= scale;
            return ToIso(coord);
        }

        public static Vector2 ToCartesian(Vector2 coord)
        {
            return new Vector2(
                (2.0f * coord.Y + coord.X) / 2.0f,
                (2.0f * coord.Y - coord.X) / 2.0f
            );
        }

        public static Vector2 ToCartesian(Vector2 coord, float scale)
        {
            Vector2 vec = ToCartesian(coord);
            return vec / scale;
        }
    }
}