using Microsoft.Xna.Framework;

namespace Tide.XMLSchema
{
    public enum bindingType
    {
        CANVAS,
        FUNCTION,
        GOTO
    }

    public struct FCinematic : ISerialisedInstanceData
    {
        public bool[] bLocksInput;
        public string[] texts;
        public bindingType[] bindingType;
        public string[] bindings;
        public string[][] highlights;
        public Vector3[][] positions;
    }
}
