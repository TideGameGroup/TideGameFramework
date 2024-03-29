﻿using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Tide.XMLSchema;

namespace Tide.Tools
{
    public class FDynamicCanvas : ISerialisedInstanceData
    {
        public List<EWidgetAlignment> alignments = new List<EWidgetAlignment>();
        public List<EWidgetAnchor> anchors = new List<EWidgetAnchor>();
        public List<string> clickSounds = new List<string>();
        public List<Color> colors = new List<Color>();
        public List<string> fonts = new List<string>();
        public List<Color> highlightColors = new List<Color>();
        public List<string> hoverSounds = new List<string>();
        public string ID = "";
        public List<string> IDs = new List<string>();
        public List<int> parents = new List<int>();
        public List<Rectangle> rectangles = new List<Rectangle>();
        public Rectangle root = Rectangle.Empty;
        public List<Rectangle> sources = new List<Rectangle>();
        public List<string> texts = new List<string>();
        public List<string> textures = new List<string>();
        public List<string> tooltips = new List<string>();
        public List<string> tooltiptexts = new List<string>();
        public List<bool> visibilities = new List<bool>();
        public List<EWidgetType> widgetTypes = new List<EWidgetType>();

        public FDynamicCanvas(string ID)
        {
            this.ID = ID;
        }

        public FDynamicCanvas(FCanvas canvas)
        {
            ID = canvas.ID;
            root = canvas.root;
            alignments = new List<EWidgetAlignment>(canvas.alignments);
            anchors = new List<EWidgetAnchor>(canvas.anchors);
            clickSounds = new List<string>(canvas.clickSounds);
            colors = new List<Color>(canvas.colors);
            fonts = new List<string>(canvas.fonts);
            highlightColors = new List<Color>(canvas.highlightColors);
            hoverSounds = new List<string>(canvas.hoverSounds);
            IDs = new List<string>(canvas.IDs);
            parents = new List<int>(canvas.parents);
            rectangles = new List<Rectangle>(canvas.rectangles);
            sources = new List<Rectangle>(canvas.sources);
            texts = new List<string>(canvas.texts);
            textures = new List<string>(canvas.textures);
            tooltips = new List<string>(canvas.tooltips);
            tooltiptexts = new List<string>(canvas.tooltiptexts);
            visibilities = new List<bool>(canvas.visibilities);
            widgetTypes = new List<EWidgetType>(canvas.widgetTypes);
        }

        public int Count => IDs.Count;

        public int Add(int prior)
        {
            IDs.Add(GetValidID(prior));
            parents.Add(prior);

            Rectangle rect = rectangles[prior];
            rect.Location = new Point(32, 32);
            rectangles.Add(rect);

            alignments.Add(alignments[prior]);
            anchors.Add(anchors[prior]);
            clickSounds.Add(clickSounds[prior]);
            colors.Add(colors[prior]);
            fonts.Add(fonts[prior]);
            highlightColors.Add(highlightColors[prior]);
            hoverSounds.Add(hoverSounds[prior]);
            sources.Add(sources[prior]);
            texts.Add("");
            textures.Add(textures[prior]);
            tooltips.Add(tooltips[prior]);
            tooltiptexts.Add(tooltiptexts[prior]);
            visibilities.Add(visibilities[prior]);
            widgetTypes.Add(widgetTypes[prior]);

            return parents.Count - 1;
        }

        public int Add(
            string ID,
            EWidgetAlignment alignment = EWidgetAlignment.L,
            EWidgetAnchor anchor = EWidgetAnchor.NW,
            string clickSound = "",
            Color color = default,
            string font = "Arial",
            Color highlightColor = default,
            string hoverSound = "",
            int parent = -1,
            Rectangle rectangle = default,
            Rectangle source = default,
            string text = "",
            string texture = "",
            string tooltip = "",
            string tooltiptext = "",
            bool visible = true,
            EWidgetType widgetType = EWidgetType.BUTTON
            )
        {
            IDs.Add(GetValidID(ID));
            alignments.Add(alignment);
            anchors.Add(anchor);
            clickSounds.Add(clickSound);
            colors.Add(color == default ? Color.White : color);
            fonts.Add(font);
            highlightColors.Add(highlightColor == default ? Color.White : highlightColor);
            hoverSounds.Add(hoverSound);
            parents.Add(parent);
            rectangles.Add(rectangle == default ? new Rectangle(0, 0, 96, 32) : rectangle);
            sources.Add(source == default ? new Rectangle(0, 0, 32, 32) : source);
            texts.Add(text);
            textures.Add(texture);
            tooltips.Add(tooltip);
            tooltiptexts.Add(tooltiptext);
            visibilities.Add(visible);
            widgetTypes.Add(widgetType);

            return parents.Count - 1;
        }

        public FCanvas AsCanvas()
        {
            return new FCanvas
            {
                ID = ID,
                root = root,
                alignments = alignments.ToArray(),
                anchors = anchors.ToArray(),
                clickSounds = clickSounds.ToArray(),
                colors = colors.ToArray(),
                fonts = fonts.ToArray(),
                highlightColors = highlightColors.ToArray(),
                hoverSounds = hoverSounds.ToArray(),
                IDs = IDs.ToArray(),
                parents = parents.ToArray(),
                rectangles = rectangles.ToArray(),
                sources = sources.ToArray(),
                texts = texts.ToArray(),
                textures = textures.ToArray(),
                tooltips = tooltips.ToArray(),
                tooltiptexts = tooltiptexts.ToArray(),
                visibilities = visibilities.ToArray(),
                widgetTypes = widgetTypes.ToArray()
            };
        }
        public int Duplicate(int prior)
        {
            IDs.Add(GetValidID(prior));
            parents.Add(parents[prior]);

            Rectangle rect = rectangles[prior];
            rect.Location = new Point(32, 32);
            rectangles.Add(rect);

            alignments.Add(alignments[prior]);
            anchors.Add(anchors[prior]);
            clickSounds.Add(clickSounds[prior]);
            colors.Add(colors[prior]);
            fonts.Add(fonts[prior]);
            highlightColors.Add(highlightColors[prior]);
            hoverSounds.Add(hoverSounds[prior]);
            sources.Add(sources[prior]);
            texts.Add("");
            textures.Add(textures[prior]);
            tooltips.Add(tooltips[prior]);
            tooltiptexts.Add(tooltips[prior]);
            visibilities.Add(visibilities[prior]);
            widgetTypes.Add(widgetTypes[prior]);

            return parents.Count - 1;
        }

        private string GetValidID(int prior)
        {
            string id = string.Format("{0}-copy", IDs[prior]);
            return GetValidID(id);
        }

        private string GetValidID(string id)
        {
            int i = 0;
            while (i < IDs.Count)
            {
                if (IDs[i] == id)
                {
                    id = string.Format("{0}-copy", id);
                    i = 0;
                }
                i++;
            }

            return id;
        }

        public void RemoveAt(int i)
        {
            alignments.RemoveAt(i);
            anchors.RemoveAt(i);
            clickSounds.RemoveAt(i);
            colors.RemoveAt(i);
            fonts.RemoveAt(i);
            highlightColors.RemoveAt(i);
            hoverSounds.RemoveAt(i);
            parents.RemoveAt(i);
            rectangles.RemoveAt(i);
            sources.RemoveAt(i);
            texts.RemoveAt(i);
            textures.RemoveAt(i);
            tooltips.RemoveAt(i);
            tooltiptexts.RemoveAt(i);
            visibilities.RemoveAt(i);
            widgetTypes.RemoveAt(i);
            IDs.RemoveAt(i);
        }
    }
}