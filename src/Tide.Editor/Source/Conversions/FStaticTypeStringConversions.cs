using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

namespace Tide.Editor
{
    public class FStaticTypeStringConversions
    {
        public static string ColorToHexcodeString(Color color)
        {
            return color.A.ToString("X2") + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
        }

        public static string[] ColorToHexcodeStringArray(Color color)
        {
            return new string[]
            {
                color.R.ToString("X2"),
                color.G.ToString("X2"),
                color.B.ToString("X2"),
                color.A.ToString("X2"),
            };
        }
        public static string[] ColorToStringArray(Color color)
        {
            return new string[]
            {
                color.R.ToString(),
                color.G.ToString(),
                color.B.ToString(),
                color.A.ToString(),
            };
        }

        public static bool HexcodeToColor(string hexcode, out Color color)
        {
            if (hexcode.Length != 8)
            {
                color = Color.White;
                return false;
            }

            return StringArrayToColor(
                new string[]
                {
                    hexcode[0..1],
                    hexcode[2..3],
                    hexcode[4..5],
                    hexcode[6..7],
                },
                out color
                );
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

        public static bool StringArrayToColor(string[] bytes, out Color color)
        {
            if (bytes.Length != 4)
            {
                color = Color.White;
                return false;
            }

            try
            {
                color = new Color(
                    byte.Parse(bytes[0]),
                    byte.Parse(bytes[1]),
                    byte.Parse(bytes[2]),
                    byte.Parse(bytes[3])
                    );

                return true;
            }
            catch (Exception e)
            {
                Debug.Write(e);
                color = Color.White;
                return false;
            }
        }
        public static bool StringArrayToRectangle(string[] ints, out Rectangle rect)
        {
            if (ints.Length != 4)
            {
                rect = Rectangle.Empty;
                return false;
            }

            try
            {
                rect = new Rectangle(
                    int.Parse(ints[0]),
                    int.Parse(ints[1]),
                    int.Parse(ints[2]),
                    int.Parse(ints[3])
                    );

                return true;
            }
            catch (Exception e)
            {
                Debug.Write(e);
                rect = Rectangle.Empty;
                return false;
            }
        }
    }
}