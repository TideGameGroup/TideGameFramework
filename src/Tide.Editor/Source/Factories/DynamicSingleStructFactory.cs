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


            string[] rectlabels = { "X", "Y", "W", "H" };
            string[] colorlabels = { "R", "G", "B", "A" };

            AddAttributeWidget(0, "ID", canvas.IDs[i]);
            AddAttributeWidget(1, "parent", canvas.parents[i].ToString());

            AddAttributeWidget(2, "widgettype", canvas.widgetTypes[i].ToString());
            //AddAttributeWidget(3, "position", FStaticTypeStringConversions.RectangleToString(canvas.rectangles[i]));
            AddAttribute4Widget(3, "position", FStaticTypeStringConversions.RectangleToStringArray(canvas.rectangles[i]), rectlabels);
            AddAttributeWidget(4, "texture", canvas.textures[i]);
            AddAttribute4Widget(5, "source", FStaticTypeStringConversions.RectangleToStringArray(canvas.sources[i]), rectlabels);

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
                rectangle: new Rectangle(109, 4 + n * 20, 282, 16),
                source: new Rectangle(240, 0, 16, 16),
                texture: "Icons",
                text: default_text,
                color: Color.WhiteSmoke,
                highlightColor: Color.White,
                widgetType: EWidgetType.textfield
                );
        }

        public void AddAttribute4Widget(int n, string ID, string[] default_texts, string[] labels)
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

            for (int i = 0; i < 4; i++)
            {

                newCanvas.Add(
                        ID + "_" + labels[i] + "_label",
                        parent: 1,
                        rectangle: new Rectangle(109 + i * 70, 4 + n * 20, 35, 20),
                        source: new Rectangle(0, 0, 16, 16),
                        texture: "Icons",
                        color: Color.Black,
                        highlightColor: Color.Black,
                        widgetType: EWidgetType.text,
                        text: labels[i]
                        );

                newCanvas.Add(
                    ID + "_" + labels[i] + "_field",
                    parent: 1,
                    rectangle: new Rectangle(129 + i * 70, 4 + n * 20, 50, 16),
                    source: new Rectangle(240, 0, 16, 16),
                    texture: "Icons",
                    text: default_texts[i],
                    color: Color.WhiteSmoke,
                    highlightColor: Color.White,
                    widgetType: EWidgetType.textfield
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
