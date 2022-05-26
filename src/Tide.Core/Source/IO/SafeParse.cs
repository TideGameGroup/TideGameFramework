using System;
using System.Diagnostics;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;

namespace Tide.Core
{
    public class SafeParse
    {
        public static bool ParseBool(XElement node, ref bool outBool, bool _default = false)
        {
            if (node != null)
            {
                return ParseBool(node.Value, ref outBool, _default);
            }
            return false;
        }

        public static bool ParseBool(string value, ref bool outBool, bool _default = false)
        {
            if (value != "")
            {
                if (bool.TryParse(value, out bool result))
                {
                    outBool = result;
                    return true;
                }
                else
                {
                    Debug.Print("unable to parse value");
                    outBool = _default; // flag invalid
                    return false;
                }
            }
            return false;
        }

        public static bool ParseDouble(XElement node, ref double outdouble, double _default = -1.0f)
        {
            if (node != null)
            {
                return ParseDouble(node.Value, ref outdouble, _default);
            }
            return false;
        }

        public static bool ParseDouble(string value, ref double outdouble, double _default = -1.0f)
        {
            if (value != "")
            {
                if (double.TryParse(value, NumberStyles.Float, new CultureInfo("en-GB"), out double result))
                {
                    outdouble = result;
                    return true;
                }
                else
                {
                    Debug.Print("unable to parse value");
                    outdouble = _default; // flag invalid
                    return false;
                }
            }
            return false;
        }

        public static bool ParseFloat(XElement node, ref float outFloat, float _default = -1.0f)
        {
            if (node != null)
            {
                return ParseFloat(node.Value, ref outFloat, _default);
            }
            return false;
        }

        public static bool ParseFloat(string value, ref float outFloat, float _default = -1.0f)
        {
            if (value != "")
            {
                if (float.TryParse(value, NumberStyles.Float, new CultureInfo("en-GB"), out float result))
                {
                    outFloat = result;
                    return true;
                }
                else
                {
                    Debug.Print("unable to parse value");
                    outFloat = _default; // flag invalid
                    return false;
                }
            }
            return false;
        }

        // XML node version
        public static bool ParseInt(XElement node, ref int outVal, int _default = -1)
        {
            if (node != null)
            {
                return ParseInt(node.Value, ref outVal, _default);
            }
            return false;
        }

        //string version
        public static bool ParseInt(string value, ref int outVal, int _default = -1)
        {
            if (value != "")
            {
                if (int.TryParse(value, NumberStyles.Integer, new CultureInfo("en-GB"), out int result))
                {
                    outVal = result;
                    return true;
                }
                else
                {
                    Debug.Print("unable to parse value");
                    outVal = _default;
                    return false;
                }
            }
            return false;
        }

        public static bool ParseSetting(string value, ref FSetting outVal)
        {
            string signifier = value.Substring(0, 1);
            string settingvalue = value.Substring(1);

            switch(signifier)
            {
                case "b":
                    outVal.heldType = 'b';
                    return ParseBool(settingvalue, ref outVal.b);

                case "f":
                    outVal.heldType = 'f';
                    return ParseFloat(settingvalue, ref outVal.f);

                case "i":
                    outVal.heldType = 'i';
                    return ParseInt(settingvalue, ref outVal.i);

                case "d":
                    outVal.heldType = 'd';
                    return ParseDouble(settingvalue, ref outVal.d);

                default:
                    break;
            }
            return false;
        }
    }
}
