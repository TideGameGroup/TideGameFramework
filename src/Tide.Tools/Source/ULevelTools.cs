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

        public ULevelTools(UContentManager content, TInput input)
        {
            FCanvasComponentConstructorArgs canvasArgs =
                new FCanvasComponentConstructorArgs
                {
                    audio = null,
                    canvas = content.Load<FCanvas>("LevelTools"),
                    content = content,
                    focus = EFocus.Console,
                    input = input,
                    scale = 1f
                };

            canvas = new ACanvasComponent(canvasArgs);

            FCanvasDrawComponentConstructorArgs canvasRenderArgs =
                new FCanvasDrawComponentConstructorArgs
                {
                    component = canvas,
                    content = content
                };

            ACanvasDrawComponent canvasDraw = new ACanvasDrawComponent(canvasRenderArgs);

            AddChildComponent(canvas);
            AddChildComponent(canvasDraw);
        }

        public void BindActionToCanvas(string action, WidgetDelegate callback)
        {
            canvas.BindAction(action, callback);
        }

        public void ExportSerialisedInstanceData(string folder, string Id, ISerialisedInstanceData data)
        {
            UExportTools.ExportSerialisedInstanceData(folder, Id, data);
        }

        public void ExportSerialisedSet(string path, ref Dictionary<string, ISerialisedInstanceData> serialisedSet)
        {
            UExportTools.ExportSerialisedSet(path, ref serialisedSet);
        }

        public static FCanvas GenerateFCanvas()
        {
            return UExportTools.GenerateBlankCanvas();
        }
    }
}
