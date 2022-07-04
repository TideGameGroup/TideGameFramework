using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Diagnostics;
using System.IO;

namespace Tide.Reflection
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(ProjectSourcePath.Path);


            string projPath = Path.Combine(ProjectSourcePath.Path, "Tide.Reflection.csproj");

            GetProjectReferences(projPath);

            foreach (var cls in GetClasses("Tide.Core", "Tide.Core"))
            {
                Console.WriteLine(cls);
            }
        }

        static IEnumerable<XElement> GetProjectReferences(string path)
        {
            XDocument doc = new XDocument();
            XML(path, ref doc);

            if (doc != null)
            {
                return doc.Descendants("ProjectReference");
            }

            return null;
        }

        static IEnumerable<string> GetClasses(string assembly, string nameSpace)
        {
            Assembly asm = Assembly.Load(assembly);
            return asm.GetTypes()
                .Where(type => type.Namespace == nameSpace)
                .Select(type => type.Name);
        }

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
    }
}
