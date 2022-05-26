using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Tide.Core;
using Tide.XMLSchema;

namespace Tide.Tools
{
    public class ULevelTools : UComponent
    {
        private readonly ACanvasComponent canvas;

        public ULevelTools(UContentManager content, UInput uInput)
        {
            AInputComponent input = new AInputComponent(uInput);
            canvas = new ACanvasComponent(content, input, GenerateFCanvas(), EFocus.Console);

            RegisterChildComponent(input);
            RegisterChildComponent(canvas);
        }

        public void BindActionToCanvas(string action, WidgetDelegate callback)
        {
            canvas.BindAction(action, callback);
        }

        public void ExportSerialisedInstanceData(string folder, string Id, ISerialisedInstanceData data)
        {
            string path = Path.Combine(folder, string.Format("{0}.xml", Id));

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            using (XmlWriter writer = XmlWriter.Create(path, settings))
            {
                IntermediateSerializer.Serialize(writer, data, null);
            }
        }

        public void ExportSerialisedSet(string path, ref Dictionary<string, ISerialisedInstanceData> serialisedSet)
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

        private FCanvas GenerateFCanvas()
        {
            FCanvas canvas = new FCanvas
            {
                ID = "ULevelToolsCanvas",
                root = new Rectangle(0, 0, 0, 0),
                anchor = EWidgetAnchor.W,

                parents         = new int[1] { -1 },
                fonts           = new string[1] { "Arial" },
                IDs             = new string[1] { "export" },
                textures        = new string[1] { "" },
                texts           = new string[1] { "" },
                tooltips        = new string[1] { "" },
                rectangles      = new Rectangle[1] { new Rectangle(0, 0, 60, 30 ) },
                alignments      = new EWidgetAlignment[1] { EWidgetAlignment.Left },
                sources         = new Rectangle[1] { new Rectangle(0, 0, 1, 1) },
                colors          = new Color[1] { Color.Gray },
                highlightColors = new Color[1] { Color.White },
                widgetTypes     = new EWidgetType[1] { EWidgetType.button },
            };

            return canvas;
        }
    }
}
