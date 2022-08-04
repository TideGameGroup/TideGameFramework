using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using Tide.Core;
using Tide.XMLSchema;

namespace Tide.Tools
{
    public class UExportTools : UComponent
    {
        public UExportTools()
        { }

        public static void ExportSerialisedInstanceData(string path, ISerialisedInstanceData data)
        {
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true
            };
            using XmlWriter writer = XmlWriter.Create(path, settings);
            IntermediateSerializer.Serialize(writer, data, null);
            writer.Close();

            Stopwatch watch = new Stopwatch();
            watch.Start();

            while (watch.ElapsedMilliseconds < 3000)
            {
                XDocument doc;
                try
                {
                    doc = XDocument.Load(path);
                    XElement node = doc.Element("XnaContent");
                    if (node != null)
                    {
                        var staticProperty = data.GetType().GetProperty("ExpectedVersion", BindingFlags.Public | BindingFlags.Static);
                        var value = staticProperty.GetValue(data, null);
                        node.SetAttributeValue("XnaVersion", value);
                    }
                    doc.Save(path);
                    return;
                }
                catch (IOException)
                {
                    continue;
                }
                catch (Exception)
                {
                    return;
                }
            }
        }

        public static void ExportSerialisedInstanceData(string folder, string Id, ISerialisedInstanceData data)
        {
            string path = Path.Combine(folder, string.Format("{0}.xml", Id));
            ExportSerialisedInstanceData(path, data);
        }

        public static void ExportSerialisedSet(string path, ref Dictionary<string, ISerialisedInstanceData> serialisedSet)
        {
            string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            string exportFolder = "Export";

            foreach (string key in serialisedSet.Keys)
            {
                if (key.StartsWith("Level_"))
                {
                    exportFolder = key;
                    break;
                }
            }

            string exportDirectory = Path.Combine(projectDirectory, path);
            exportDirectory = Path.Combine(exportDirectory, exportFolder);

            if (!Directory.Exists(exportDirectory))
            {
                Directory.CreateDirectory(exportDirectory);
            }

            foreach (string key in serialisedSet.Keys)
            {
                ISerialisedInstanceData data = serialisedSet[key];
                ExportSerialisedInstanceData(exportDirectory, key, data);
            }
        }

        public static FCanvas GenerateBlankCanvas()
        {
            FCanvas canvas = new FCanvas
            {
                ID = "UExportToolsCanvas",
                anchors = new EWidgetAnchor[1] { EWidgetAnchor.C },
                clickSounds = new string[1] { "" },
                parents = new int[1] { -1 },
                fonts = new string[1] { "Arial" },
                IDs = new string[1] { "export" },
                hoverSounds = new string[1] { "" },
                textures = new string[1] { "" },
                texts = new string[1] { "textfield" },
                tooltips = new string[1] { "" },
                rectangles = new Rectangle[1] { new Rectangle(-100, -30, 100, 30) },
                alignments = new EWidgetAlignment[1] { EWidgetAlignment.L },
                sources = new Rectangle[1] { new Rectangle(0, 0, 1, 1) },
                colors = new Color[1] { Color.Gray },
                highlightColors = new Color[1] { Color.White },
                widgetTypes = new EWidgetType[1] { EWidgetType.TEXTFIELD },
            };

            return canvas;
        }
    }
}