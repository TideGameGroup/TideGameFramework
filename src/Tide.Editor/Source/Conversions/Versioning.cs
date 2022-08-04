using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Tide.Core;
using Tide.XMLSchema;

namespace Tide.Editor
{
    public enum EVersioningResult
    {
        EFILEERROR,
        ENOTXNA,
        ETYPEERROR,
        ENOVERSION,
        EPARSEERROR,
        ENOCONVERSION,
        ESUCCESS
    }

    public struct FVersioningInfo
    {
        public EVersioningResult result;
        public float version;
        public Type type;
    }

    class Versioning
    {
        public static FVersioningInfo CheckVersioning(string path, out XDocument xml)
        {
            if (SafeLoad.XML(path, out xml))
            {
                XElement node = xml.Element("XnaContent");
                if (node == null)
                {
                    return new FVersioningInfo { result = EVersioningResult.ENOTXNA };
                }

                if (!GetType(xml, out Type type))
                {
                    return new FVersioningInfo { result = EVersioningResult.ETYPEERROR };
                }

                XAttribute attr = node.Attribute("XnaVersion");
                if (attr == null)
                {
                    return new FVersioningInfo { 
                        result = EVersioningResult.ESUCCESS,
                        version = 0f,
                        type = type
                    };
                }

                if (SafeParse.ParseFloat(attr, out float version))
                {
                    return new FVersioningInfo
                    {
                        result = EVersioningResult.ESUCCESS,
                        version = version,
                        type = type
                    };
                }
                return new FVersioningInfo { result = EVersioningResult.EPARSEERROR };
            }
            return new FVersioningInfo { result = EVersioningResult.EFILEERROR };
        }

        private static bool GetType(XDocument xml, out Type type)
        {
            XElement xnacontent = xml.Element("XnaContent");
            XElement asset = xnacontent.Element("Asset");

            if (GetValidTypeString(xnacontent, asset, out string typeString))
            {
                Type clsType = Type.GetType(typeString);

                if (clsType != null)
                {
                    type = clsType;
                    return true;
                }
            }

            type = null;
            return false;
        }

        private static bool GetValidTypeString(XElement xnacontent, XElement asset, out string typeString)
        {
            typeString = "";

            XAttribute type = asset.Attribute("Type");
            XAttribute nms = xnacontent.Attributes().FirstOrDefault(x => x.Name.LocalName == "ns");

            if (nms != null && nms.Value == "Microsoft.Xna.Framework")
            {
                typeString = type.Value;
                typeString = typeString.Replace(":", ".");
                typeString += ", " + nms.Value;
                return true;
            }

            nms = xnacontent.Attributes().FirstOrDefault(x => x.Name.LocalName == "XMLSchema");

            if (nms != null)
            {
                typeString = nms.Value;
                typeString = typeString.Replace("XMLSchema", type.Value);
                typeString = typeString.Replace(":", ".");
                typeString += ", " + nms.Value;
                return true;
            }

            return false;
        }

        public static void DoConversionChain(FVersioningInfo res, ref XDocument xml)
        {
            if (res.type == typeof(FCanvas))
            {
                CanvasVersioning.DoConversionChain(res.version, ref xml);
            }
        }
    }
}
