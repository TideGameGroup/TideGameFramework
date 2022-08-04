using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Tide.Core;
using Tide.Tools;
using Tide.XMLSchema;

namespace Tide.Editor
{
    public class DynamicLibraryFactory : ITreeCanvasFactory
    {
        private FDynamicCanvas newCanvas = null;

        public DynamicLibraryFactory(UContentManager content, int height)
        {
            newCanvas = new FDynamicCanvas("Tree");
            newCanvas.root = new Rectangle(0, 24, 0, 0);
            ITreeCanvasFactory.AddTreePanel(newCanvas, height);

            int placeoffset = 0;

            foreach (var key in content.DynamicLibrary.Keys)
            {
                newCanvas.Add(
                   "text" + key,
                    widgetType: EWidgetType.TEXT,
                    alignment: EWidgetAlignment.L,
                    rectangle: new Rectangle(40, 24 + placeoffset * 20, 100, 16),
                    source: new Rectangle(0, 0, 0, 0),
                    text: key,
                    font: "consolas",
                    color: Color.DarkSlateGray,
                    highlightColor: Color.DarkSlateGray
                 );

                placeoffset++;
            }
        }

        public FCanvas GetCanvas()
        {
            return newCanvas.AsCanvas();
        }

        public FDynamicCanvas GetDynamicCanvas()
        {
            return newCanvas;
        }
    }
}