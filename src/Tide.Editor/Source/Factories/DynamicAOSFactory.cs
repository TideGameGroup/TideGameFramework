using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Tide.Tools;
using Tide.XMLSchema;

namespace Tide.Editor
{
    public class DynamicAOSFactory : ITreeCanvasFactory
    {
        readonly FDynamicCanvas canvas = null;
        FDynamicCanvas newCanvas = null;

        public DynamicAOSFactory(FDynamicCanvas canvas)
        {
            this.canvas = canvas;

            newCanvas = new FDynamicCanvas("Tree");
            newCanvas.root = new Rectangle(32, 32, 0, 0);

            for (int i = 0; i < canvas.alignments.Count; i++)
            {
                // use a single struct factory to build a single struct canvas
                // add to dynamic canvas

                newCanvas.Add(
                    "button" + i.ToString(), 
                    rectangle: new Rectangle(0, i * 32, 100, 16),
                    source: new Rectangle(0, 0, 200, 16),
                    texture: "Icons",
                    color: Color.White,
                    highlightColor: Color.White
                    );
            }
        }

        public FDynamicCanvas GetDynamicCanvas()
        {
            return newCanvas;
        }

        public FCanvas GetCanvas()
        {
            return newCanvas.AsCanvas();
        }
    }
}
