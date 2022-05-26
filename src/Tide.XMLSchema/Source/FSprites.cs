using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tide.XMLSchema
{
    public struct FSprites: ISerialisedInstanceData
    {
        //public string[] animations;
        public bool[]   bShouldDraw;
        public Color[]  colors;
        public string[] names;
        public float[]  scales;
    }
}
