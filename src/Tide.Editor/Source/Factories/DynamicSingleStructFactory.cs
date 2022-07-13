using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Tide.Tools;
using Tide.XMLSchema;

namespace Tide.Editor
{
    public class DynamicSingleStructFactory : ITreeCanvasFactory
    {
        readonly FDynamicCanvas canvas = null;
        FDynamicCanvas newCanvas = null;

        public DynamicSingleStructFactory(FDynamicCanvas canvas, int i)
        {
            this.canvas = canvas;

            newCanvas = new FDynamicCanvas("Properties");
            newCanvas.root = new Rectangle(0, 0, 0, 0);

            newCanvas.Add(
                    "property_panel_1",
                    anchor: EWidgetAnchor.SW,
                    rectangle: new Rectangle(0, -400, 400, 400),
                    source: new Rectangle(240, 0, 16, 16),
                    texture: "Icons",
                    color: Color.DarkGray,
                    highlightColor: Color.DarkGray
                    );

            newCanvas.Add(
                    "property_panel_2",
                    parent: 0,
                    rectangle: new Rectangle(1, 1, 398, 398),
                    source: new Rectangle(240, 0, 16, 16),
                    texture: "Icons",
                    color: Color.LightGray,
                    highlightColor: Color.LightGray
                    );

            AddAttributeWidget(0, "ID", canvas.IDs[i]);
            AddAttributeWidget(1, "parent", canvas.parents[i].ToString());

            AddAttributeWidget(2, "widgettype", canvas.widgetTypes[i].ToString());
            AddAttributeWidget(3, "position", FStaticTypeStringConversions.RectangleToString(canvas.rectangles[i]));
            AddAttributeWidget(4, "texture", canvas.textures[i]);
            AddAttributeWidget(5, "source", FStaticTypeStringConversions.RectangleToString(canvas.sources[i]));

            AddAttributeWidget(6, "alignment", canvas.alignments[i].ToString());
            AddAttributeWidget(7, "anchor", canvas.anchors[i].ToString());

            AddAttributeWidget(8, "text", canvas.texts[i]);
            AddAttributeWidget(9, "font", canvas.fonts[i]);
            AddAttributeWidget(10, "color", FStaticTypeStringConversions.ColorToHexcodeString(canvas.colors[i]));
            AddAttributeWidget(11, "highlightcolor", FStaticTypeStringConversions.ColorToHexcodeString(canvas.highlightColors[i]));

            AddAttributeWidget(12, "clicksound", canvas.clickSounds[i]);
            AddAttributeWidget(13, "hoversound", canvas.hoverSounds[i]);

            AddAttributeWidget(14, "tooltip", canvas.tooltips[i]);
        }

        public void AddAttributeWidget(int n, string ID, string default_text)
        {
            newCanvas.Add(
                    ID + "_label",
                    parent: 1,
                    rectangle: new Rectangle(4, 4 + n * 20, 100, 20),
                    source: new Rectangle(0, 0, 16, 16),
                    texture: "Icons",
                    color: Color.Black,
                    highlightColor: Color.Black,
                    widgetType: EWidgetType.text,
                    text: ID + ":"
                    );

            newCanvas.Add(
                ID + "_field",
                parent: 1,
                rectangle: new Rectangle(109, 4 + n * 20, 282, 18),
                source: new Rectangle(240, 0, 16, 16),
                texture: "Icons",
                text: default_text,
                color: Color.WhiteSmoke,
                highlightColor: Color.White,
                widgetType: EWidgetType.textfield
                );
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
