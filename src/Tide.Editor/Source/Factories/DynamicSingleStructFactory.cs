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

            if (canvas.Count > 0 && i >= 0 && i < canvas.Count)
            {
                AddAttributeWidget(0, "ID", canvas.IDs[i]);
                AddAttributeWidget(1, "parent", canvas.parents[i].ToString());

                AddAttributeWidget(2, "widgettype", canvas.widgetTypes[i].ToString(),
                    "valid values:\nPANEL\nBUTTON\nTEXT\nSLIDER\nTICKBOX\nSCROLLBAR\nTEXTFIELD");

                AddAttributeWidget(3, "texture", canvas.textures[i]);
                AddAttribute4Widget(4, "position", FStaticTypeStringConversions.RectangleToStringArray(canvas.rectangles[i]), rectlabels);
                AddAttribute4Widget(5, "source", FStaticTypeStringConversions.RectangleToStringArray(canvas.sources[i]), rectlabels);

                AddAttributeWidget(6, "alignment", canvas.alignments[i].ToString(),
                    "valid values:\nC\nL\nR");
                AddAttributeWidget(7, "anchor", canvas.anchors[i].ToString(),
                    "valid values:\nN\nNE\nE\nSE\nS\nSW\nW\nNW\nC");

                AddAttributeWidget(8, "text", canvas.texts[i]);
                AddAttributeWidget(9, "font", canvas.fonts[i]);
                AddAttribute4Widget(10, "color", FStaticTypeStringConversions.ColorToStringArray(canvas.colors[i]), colorlabels);
                AddAttribute4Widget(11, "highlightcolor", FStaticTypeStringConversions.ColorToStringArray(canvas.highlightColors[i]), colorlabels);

                AddAttributeWidget(12, "clicksound", canvas.clickSounds[i]);
                AddAttributeWidget(13, "hoversound", canvas.hoverSounds[i]);

                AddAttributeWidget(14, "tooltip", canvas.tooltips[i]);
            }
        }

        public void AddAttributeWidget(int n, string ID, string default_text, string tooltiptext = "")
        {
            newCanvas.Add(
                    ID + "_label",
                    parent: 1,
                    rectangle: new Rectangle(4, 4 + n * 20, 100, 20),
                    source: new Rectangle(0, 0, 16, 16),
                    texture: "Icons",
                    color: Color.DarkSlateGray,
                    highlightColor: Color.DarkSlateGray,
                    widgetType: EWidgetType.TEXT,
                    text: ID + ":",
                    font: "consolas"
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
                widgetType: EWidgetType.TEXTFIELD,
                font: "consolas",
                tooltip: tooltiptext != "" ? "Tooltip" : "",
                tooltiptext: tooltiptext
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
                    color: Color.DarkSlateGray,
                    highlightColor: Color.DarkSlateGray,
                    widgetType: EWidgetType.TEXT,
                    text: ID + ":",
                    font: "consolas"
                    );

            for (int i = 0; i < 4; i++)
            {

                newCanvas.Add(
                    ID + "_" + labels[i] + "_label",
                    parent: 1,
                    rectangle: new Rectangle(109 + i * 70, 4 + n * 20, 35, 20),
                    source: new Rectangle(0, 0, 16, 16),
                    texture: "Icons",
                    color: Color.DarkSlateGray,
                    highlightColor: Color.DarkSlateGray,
                    widgetType: EWidgetType.TEXT,
                    text: labels[i],
                    font: "consolas"
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
                    widgetType: EWidgetType.TEXTFIELD,
                    font: "consolas"
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
