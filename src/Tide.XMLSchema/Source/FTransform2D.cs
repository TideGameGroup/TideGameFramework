using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Tide.XMLSchema
{
    public struct FTransform2D : ISerialisedInstanceData
    {
        public Vector3[]    positions;
        public float[]      angles;
        public float[]      scales;

        public string Type => "Tide.Core.ATransform2D";
    }
}
