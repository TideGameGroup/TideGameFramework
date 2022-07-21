using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Tide.Core;

namespace Tide.Editor
{
    public enum EVersioningResult
    {
        EFILEERROR,
        ENOTXNA,
        ENOVERSION,
        EPARSEERROR,
        ELESSERVERSION,
        EGREATERVERSION,
        ESUCCESS
    }

    class Versioning
    {
        public EVersioningResult CheckVersioning(string path, float expectedVersion, out XDocument xml)
        {
            if (SafeLoad.XML(path, out xml))
            {
                XElement node = xml.Element("XnaContent");

                if (node == null)
                {
                    return EVersioningResult.ENOTXNA;
                }

                XAttribute attr = node.Attribute("xnaversion");

                if (attr == null)
                {
                    return EVersioningResult.ENOVERSION;
                }

                if (SafeParse.ParseFloat(attr, out float version))
                {
                    return CompareVersions(expectedVersion, version);
                }

                return EVersioningResult.EPARSEERROR;
            }

            return EVersioningResult.EFILEERROR;
        }

        public EVersioningResult CompareVersions(float expected, float received)
        {
            if (expected > received)
            {
                return EVersioningResult.EGREATERVERSION;
            }
            if (expected < received)
            {
                return EVersioningResult.ELESSERVERSION;
            }
            
            return EVersioningResult.ESUCCESS;
        }
    }
}
