using Microsoft.Xna.Framework;
using System;

namespace Tide.Core
{
    public struct FIntVector2
    {
        public int x;
        public int y;

        public FIntVector2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public FIntVector2(Vector3 vector3)
        {
            x = (int)MathF.Round(vector3.X);
            y = (int)MathF.Round(vector3.Z);
        }

        public FIntVector2(Vector2 vector2)
        {
            x = (int)MathF.Round(vector2.X);
            y = (int)MathF.Round(vector2.Y);
        }

        public Vector2 AsVector2()
        { return new Vector2(x, y); }

        public Vector3 AsVector3()
        { return new Vector3(x, 0, y); }

        public bool IsZero()
        { return x == 0 && y == 0; }

        #region overrides

        public override bool Equals(object obj)
        {
            return obj is FIntVector2 vector &&
                   x == vector.x &&
                   y == vector.y;
        }

        // overrides
        public override int GetHashCode()
        {
            return HashCode.Combine(x, y);
        }

        #endregion overrides

        #region Static

        public static float Distance(FIntVector2 A, FIntVector2 B)
        {
            return MathF.Sqrt(MathF.Pow(A.x - B.x, 2.0f) + MathF.Pow(A.y - B.y, 2.0f));
        }

        public static float DistanceNonEuclidean(FIntVector2 A, FIntVector2 B)
        {
            return MathF.Min(A.x - B.x, A.y - B.y);
        }

        public static FIntVector2 Lerp(FIntVector2 A, FIntVector2 B, float a)
        {
            Vector2 AV = A.AsVector2();
            Vector2 BV = B.AsVector2();
            return new FIntVector2(Vector2.Lerp(AV, BV, a));
        }

        public static FIntVector2 operator + (FIntVector2 A, FIntVector2 B)
        {
            return new FIntVector2(A.x + B.x, A.y + B.y);
        }

        public static bool operator !=(FIntVector2 A, FIntVector2 B)
        {
            return !(A == B);
        }

        public static bool operator ==(FIntVector2 A, FIntVector2 B)
        {
            return (A.x == B.x) && (A.y == B.y);
        }

        public static FIntVector2 RandomUnitVector(Random rnd = null)
        {
            rnd = (rnd == null) ? new Random() : rnd;
            int r = rnd.Next(0, 9);
            int rx = (r % 3) - 1;
            int ry = (r / 3) - 1;
            return new FIntVector2(rx, ry);
        }

        #endregion Static
    }
}