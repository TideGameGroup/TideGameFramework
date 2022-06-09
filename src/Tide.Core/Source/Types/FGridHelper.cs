using System;
using Tide.Core;

namespace Grim.Core
{
    public class FGridHelper
    {
        public static int Get1DIndex(FIntVector2 coord, int width)
        {
            return (coord.y * width) + coord.x;
        }

        public static int Get1DIndex(int x, int y, int width)
        {
            return (y * width) + x;
        }

        public static FIntVector2 Get2DIndex(int i, int width, int height)
        {
            int y = Math.Clamp(i / width, 0, height - 1);
            int x = i % width;
            return new FIntVector2(x, y);
        }

        public static bool IsInBounds(FIntVector2 pos, int width, int height)
        {
            return (pos.x >= 0 && pos.x < width) && (pos.y >= 0 && pos.y < height);
        }

        public static bool IsValidIndex(int i, int width, int height)
        {
            return (i >= 0 && i < height * width);
        }
    }
}