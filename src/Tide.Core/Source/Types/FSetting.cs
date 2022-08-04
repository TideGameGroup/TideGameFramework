using System.Runtime.InteropServices;

namespace Tide.Core
{
    [System.Runtime.InteropServices.StructLayout(LayoutKind.Explicit)]
    public struct FSetting
    {
        [System.Runtime.InteropServices.FieldOffset(0)]
        public bool b;

        [System.Runtime.InteropServices.FieldOffset(0)]
        public int i;

        [System.Runtime.InteropServices.FieldOffset(0)]
        public double d;

        [System.Runtime.InteropServices.FieldOffset(0)]
        public float f;

        [System.Runtime.InteropServices.FieldOffset(sizeof(double))]
        public char heldType;

        public static FSetting Bool(bool v)
        { var s = new FSetting(); s.heldType = 'b'; s.b = v; return s; }

        public static FSetting Int(int v)
        { var s = new FSetting(); s.heldType = 'i'; s.i = v; return s; }

        public static FSetting Double(double v)
        { var s = new FSetting(); s.heldType = 'd'; s.d = v; return s; }

        public static FSetting Float(float v)
        { var s = new FSetting(); s.heldType = 'f'; s.f = v; return s; }

        public string GetValueString()
        {
            return heldType switch
            {
                'b' => "b" + b.ToString(),
                'f' => "f" + f.ToString(),
                'i' => "i" + i.ToString(),
                'd' => "d" + d.ToString(),
                _ => "",
            };
        }
    }
}