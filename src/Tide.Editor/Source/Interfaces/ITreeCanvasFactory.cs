using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Tide.Tools;
using Tide.XMLSchema;

namespace Tide.Editor
{
    interface ITreeCanvasFactory
    {
        public FDynamicCanvas GetDynamicCanvas();

        public FCanvas GetCanvas();

        public static void AddTreePanel(FDynamicCanvas newCanvas, int height)
        {
            newCanvas.Add(
                    "tree_panel_1",
                    anchor: EWidgetAnchor.NW,
                    rectangle: new Rectangle(0, 0, 400, height - 400 - 16),
                    source: new Rectangle(240, 0, 16, 16),
                    texture: "Icons",
                    color: Color.DarkGray,
                    highlightColor: Color.DarkGray
                    );

            newCanvas.Add(
                    "tree_panel_2",
                    parent: 0,
                    rectangle: new Rectangle(1, 1, 398, height - 398 - 16),
                    source: new Rectangle(240, 0, 16, 16),
                    texture: "Icons",
                    color: Color.LightGray,
                    highlightColor: Color.LightGray
                    );

            newCanvas.Add(
                    "tree_button",
                    widgetType: EWidgetType.BUTTON,
                    parent: 1,
                    anchor: EWidgetAnchor.NE,
                    rectangle: new Rectangle(-36, 1, 16, 16),
                    source: new Rectangle(0, 112, 16, 16),
                    texture: "Icons",
                    color: Color.LightGray,
                    highlightColor: Color.White
                    );

            newCanvas.Add(
                    "library_button",
                    widgetType: EWidgetType.BUTTON,
                    parent: 1,
                    anchor: EWidgetAnchor.NE,
                    rectangle: new Rectangle(-18, 1, 16, 16),
                    source: new Rectangle(0, 96, 16, 16),
                    texture: "Icons",
                    color: Color.LightGray,
                    highlightColor: Color.White
                    );
        }
    }
}
