using System.Collections.Generic;
using Tide.Core;

namespace Tide.Core
{
    public class FGrid<T> where T : struct
    {
        private readonly List<T> data = new List<T>();

        public FGrid(int width, int height)
        {
            Height = height;
            Width = width;

            for (int i = 0; i < Height * Width; i++)
            {
                data.Add(default);
            }
        }

        public int Height { get; set; }
        public int Width { get; set; }

        public T GetAt(FIntVector2 coord)
        {
            //if (IsValidIndex(coord))
           // {
                return data[FStaticGridFunctions.Get1DIndex(coord, Width)];
           //}
            //return default;
        }

        public T GetAt(int x, int y)
        {
            if (IsValidIndex(x, y))
            {
                return data[FStaticGridFunctions.Get1DIndex(x, y, Width)];
            }
            return default;
        }

        public T GetAt(int i)
        {
            if (IsValidIndex(i))
            {
                return data[i];
            }
            return default;
        }

        public void SetAt(FIntVector2 coord, T val)
        {
            data[FStaticGridFunctions.Get1DIndex(coord, Width)] = val;
        }

        public void SetAt(int x, int y, T val)
        {
            data[FStaticGridFunctions.Get1DIndex(x, y, Width)] = val;
        }

        public void SetAt(int i, T val)
        {
            data[i] = val;
        }

        public bool IsInBounds(FIntVector2 pos)
        {
            return FStaticGridFunctions.IsInBounds(pos, Width, Height);
        }

        public bool IsValidIndex(int i)
        {
            return FStaticGridFunctions.IsValidIndex(i, Width, Height);
        }

        public bool IsValidIndex(FIntVector2 coord)
        {
            int i = FStaticGridFunctions.Get1DIndex(coord, Width);
            return FStaticGridFunctions.IsValidIndex(i, Width, Height);
        }

        public bool IsValidIndex(int x, int y)
        {
            int i = FStaticGridFunctions.Get1DIndex(x, y, Width);
            return FStaticGridFunctions.IsValidIndex(i, Width, Height);
        }
    }
}