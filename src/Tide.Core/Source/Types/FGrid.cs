using System.Collections.Generic;
using Tide.Core;

namespace Grim.Core
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
            return data[FGridHelper.Get1DIndex(coord, Width)];
        }

        public T GetAt(int x, int y)
        {
            return data[FGridHelper.Get1DIndex(x, y, Width)];
        }

        public T GetAt(int i)
        {
            return data[i];
        }

        public void SetAt(FIntVector2 coord, T val)
        {
            data[FGridHelper.Get1DIndex(coord, Width)] = val;
        }

        public void SetAt(int x, int y, T val)
        {
            data[FGridHelper.Get1DIndex(x, y, Width)] = val;
        }

        public void SetAt(int i, T val)
        {
            data[i] = val;
        }
        
        public bool IsInBounds(FIntVector2 pos)
        {
            return FGridHelper.IsInBounds(pos, Width, Height);
        }

        public bool IsValidIndex(int i)
        {
            return FGridHelper.IsValidIndex(i, Width, Height);
        }
    }
}