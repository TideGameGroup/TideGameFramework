using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Tide.Core;
using Tide.XMLSchema;

namespace Tide.Editor
{
    class CanvasVersioning
    {
        public static void DoConversionChain(float version, ref XDocument xml)
        {
            float current = version;

            if (current < 0.1f)
            {
                V0ToV01Conversion(ref xml);
            }
        }

        private static void V0ToV01Conversion(ref XDocument xml)
        {
            XElement xnacontent = xml.Element("XnaContent");
            XElement asset = xnacontent.Element("Asset");
            XElement parents = asset.Element("parents");
            XElement tooltiptexts = asset.Element("tooltiptexts");

            int count = parents.Value.Split(" ").Count();
            string value = "";

            for (int i = 0; i < count; i++)
            {
                value += "true ";
            }

            XElement elem = new XElement("visibilities") { Value = value};
            tooltiptexts.AddAfterSelf(elem);
        }
    }
}
