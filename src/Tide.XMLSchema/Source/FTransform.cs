using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Tide.XMLSchema
{
    public struct FTransform : ISerialisedInstanceData
    {
        public Vector2[]    positions;
        public float[]      angles;
        public float[]      scales;
    }
}
