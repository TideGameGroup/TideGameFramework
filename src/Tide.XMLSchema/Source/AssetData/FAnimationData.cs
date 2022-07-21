using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tide.XMLSchema
{
    public struct FAnimationData
    {
        public string texID;
        public int frames;
        public int width;
        public bool loops;
        public Rectangle source;
        public float frameRate;
    }
}
