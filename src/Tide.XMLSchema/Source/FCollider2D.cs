using System.Runtime.InteropServices;
using Tide.XMLSchema;

namespace Tide.XMLSchema
{
    public struct FAABB
    {
        public float height;
        public float width;
        public float xExtent;
        public float yExtent;

        public FAABB(float width, float height)
        {
            this.width = width;
            this.height = height;

            xExtent = width / 2;
            yExtent = height / 2;
        }

        public static FAABB Unit => new FAABB(1.0f, 1.0f);
    }

    public struct FCircle
    {
        public float radius;

        public FCircle(float radius)
        {
            this.radius = radius;
        }

        public static FCircle Unit => new FCircle(1.0f);
    }

    public struct FCone
    {
        public float angle;
        public float direction;
        public float length;
    }

    public struct FRectangle
    {
        public float radius;

        public FRectangle(float radius)
        {
            this.radius = radius;
        }
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
        public FAABB aabb;

        [FieldOffset(0)]
        public FCircle circle;

        [FieldOffset(0)]
        public FCone cone;

        [FieldOffset(0)]
        public FRectangle rectangle;
    }
    public enum ECollider2DType
    {
        EAABB,
        ECIRCLE,
        ECONE,
        ERECTANGLE
    }

    public struct FCollider2D : ISerialisedInstanceData
    {
        public ECollider2DType[] colliderTypes;
        public uint[] layers;
        public uint[] masks;
        public ShapeUnion[] shapes;
    }
}
