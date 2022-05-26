using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Tide.Core
{
    public class SafeLoad
    {
        static public bool XML(string path, ref XDocument outXDocument)
        {
            XDocument doc;
            try
            {
                doc = XDocument.Load(path);
            }
            catch (Exception ex)
            {
                Debug.Print(ex.ToString());
                doc = null;
            }

            if (doc != null)
            {
                outXDocument = doc;
                return true;
            }
            return false;
        }

        static public bool Text(string path, ref List<string> outTxt)
        {
            if (!File.Exists(path))
            {
                Debug.Print("error: Unable to find file:" + path);
                return false;
            }

            string file = File.ReadAllText(path);
            string[] lines = file.Split("\n"[0]);

            for (int i = 0; i < lines.Count(); i++)
            {
                lines[i] = lines[i].TrimEnd('\r', '\n', ' ');
            }
            outTxt = new List<string>(lines);
            return true;
        }
    }
}

