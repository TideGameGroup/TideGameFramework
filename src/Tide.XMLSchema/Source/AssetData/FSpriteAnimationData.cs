using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Tide.XMLSchema
{
    public struct FSpriteAnimationData
    {
        public string name;
        public string texture;
        public int frame;
        public int width;
        public bool loop;
        public Rectangle source;
        public float framerate;
    }
}
