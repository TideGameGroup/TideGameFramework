using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tide.Editor
{
    public class FStaticTypeStringConversions
    {
        public static string ColorToHexcodeString(Color color)
        {
            return color.A.ToString("X2") + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
        }

        public static string RectangleToString(Rectangle rectangle)
        {
            return rectangle.X.ToString() + " "
                + rectangle.Y.ToString() + " "
                + rectangle.Width.ToString() + " "
                + rectangle.Height.ToString();
        }

        public static string[] RectangleToStringArray(Rectangle rectangle)
        {
            string[] rtn = { 
                rectangle.X.ToString(),
                rectangle.Y.ToString(),
                rectangle.Width.ToString(),
                rectangle.Height.ToString()
            };

            return rtn;
        }
    }
}
