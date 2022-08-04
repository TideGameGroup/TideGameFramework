using System;
using System.Diagnostics;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;

namespace Tide.Core
{
    public class SafeParse
    {
        public static bool ParseBool(XAttribute attr, out bool result, bool _default = false)
        {
            if (attr != null)
            {
                return ParseBool(attr.Value, out result, _default);
            }

            result = _default;
            return false;
        }

        public static bool ParseBool(XElement node, out bool result, bool _default = false)
        {
            if (node != null)
            {
                return ParseBool(node.Value, out result, _default);
            }
            result = _default;
            return false;
        }

        public static bool ParseBool(string value, out bool result, bool _default = false)
        {
            if (value != "")
            {
                if (bool.TryParse(value, out result))
                {
                    return true;
                }
                else
                {
                    Debug.Print("unable to parse value");
                    result = _default; // flag invalid
                    return false;
                }
            }
            result = _default;
            return false;
        }

        public static bool ParseDouble(XAttribute attr, out double result, double _default = -1.0f)
        {
            if (attr != null)
            {
                return ParseDouble(attr.Value, out result, _default);
            }
            result = _default;
            return false;
        }

        public static bool ParseDouble(XElement node, out double result, double _default = -1.0f)
        {
            if (node != null)
            {
                return ParseDouble(node.Value, out result, _default);
            }
            result = default;
            return false;
        }

        public static bool ParseDouble(string value, out double result, double _default = -1.0f)
        {
            if (value != "")
            {
                if (double.TryParse(value, NumberStyles.Float, new CultureInfo("en-GB"), out result))
                {
                    return true;
                }
                else
                {
                    Debug.Print("unable to parse value");
                    result = _default; // flag invalid
                    return false;
                }
            }
            result = _default;
            return false;
        }

        public static bool ParseFloat(XAttribute attr, out float result, float _default = -1.0f)
        {
            if (attr != null)
            {
                return ParseFloat(attr.Value, out result, _default);
            }
            result = _default;
            return false;
        }

        public static bool ParseFloat(XElement node, out float result, float _default = -1.0f)
        {
            if (node != null)
            {
                return ParseFloat(node.Value, out result, _default);
            }
            result = _default;
            return false;
        }

        public static bool ParseFloat(string value, out float result, float _default = -1.0f)
        {
            if (value != "")
            {
                if (float.TryParse(value, NumberStyles.Float, new CultureInfo("en-GB"), out result))
                {
                    return true;
                }
                else
                {
                    Debug.Print("unable to parse value");
                    result = _default; // flag invalid
                    return false;
                }
            }
            result = _default;
            return false;
        }

        // XML node version
        public static bool ParseInt(XAttribute attr, out int result, int _default = -1)
        {
            if (attr != null)
            {
                return ParseInt(attr.Value, out result, _default);
            }
            result = _default;
            return false;
        }

        public static bool ParseInt(XElement node, out int result, int _default = -1)
        {
            if (node != null)
            {
                return ParseInt(node.Value, out result, _default);
            }
            result = _default;
            return false;
        }


        //string version
        public static bool ParseInt(string value, out int result, int _default = -1)
        {
            if (value != "")
            {
                if (int.TryParse(value, NumberStyles.Integer, new CultureInfo("en-GB"), out result))
                {
                    return true;
                }
                else
                {
                    Debug.Print("unable to parse value");
                    result = _default;
                    return false;
                }
            }
            result = _default;
            return false;
        }

        public static bool ParseSetting(string value, out FSetting result)
        {
            string signifier = value.Substring(0, 1);
            string settingvalue = value.Substring(1);

            result = FSetting.Bool(false);

            switch (signifier)
            {
                case "b":
                    result.heldType = 'b';
                    return ParseBool(settingvalue, out result.b);


                case "f":
                    result.heldType = 'f';
                    return ParseFloat(settingvalue, out result.f);

                case "i":
                    result.heldType = 'i';
                    return ParseInt(settingvalue, out result.i);

                case "d":
                    result.heldType = 'd';
                    return ParseDouble(settingvalue, out result.d);

                default:
                    break;
            }

            return false;
        }
    }
}
