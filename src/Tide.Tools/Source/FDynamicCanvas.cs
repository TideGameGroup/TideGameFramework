using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Tide.XMLSchema;

namespace Tide.Tools
{
    public class FDynamicCanvas : ISerialisedInstanceData
    {
        public static int staticCount = 0;

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
            widgetTypes = new List<EWidgetType>(canvas.widgetTypes);
        }

        public void DynamicAdd<T>(List<T> L, T _default)
        {
            L.Add(L.Count > 0 ? L[^1] : _default);
        }

        public void Add()
        {
            DynamicAdd(alignments, EWidgetAlignment.Left);
            DynamicAdd(anchors, EWidgetAnchor.NW);
            DynamicAdd(clickSounds, "");
            DynamicAdd(colors, Color.LightGray);
            DynamicAdd(fonts, "Arial");
            DynamicAdd(highlightColors, Color.White);
            DynamicAdd(hoverSounds, "");
            DynamicAdd(parents, -1);
            DynamicAdd(rectangles, new Rectangle(0,0,100, 30));
            DynamicAdd(sources, new Rectangle(0, 0, 100, 30));
            DynamicAdd(texts, "");
            DynamicAdd(textures, "");
            DynamicAdd(tooltips, "");
            DynamicAdd(widgetTypes, EWidgetType.button);

            IDs.Add(string.Format(IDs.Count > 0 ? IDs[^1] + "{0}" : "widget{0}", staticCount++));
        }

        public void Add(
            string ID,
            EWidgetAlignment alignment = EWidgetAlignment.Left, 
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
            EWidgetType widgetType = EWidgetType.button
            )
        {
            IDs.Add(ID);
            alignments.Add(alignment);
            anchors.Add(anchor);
            clickSounds.Add(clickSound);
            colors.Add(color);
            fonts.Add(font);
            highlightColors.Add(highlightColor);
            hoverSounds.Add(hoverSound);
            parents.Add(parent);
            rectangles.Add(rectangle);
            sources.Add(source);
            texts.Add(text);
            textures.Add(texture);
            tooltips.Add(tooltip);
            widgetTypes.Add(widgetType);
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
                widgetTypes = widgetTypes.ToArray()
            };
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
            widgetTypes.RemoveAt(i);
            IDs.RemoveAt(i);
        }
    }
}