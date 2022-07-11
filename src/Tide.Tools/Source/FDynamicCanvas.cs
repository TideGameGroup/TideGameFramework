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
        public List<Rectangle> sources = new List<Rectangle>();
        public List<string> texts = new List<string>();
        public List<string> textures = new List<string>();
        public List<string> tooltips = new List<string>();
        public List<EWidgetType> widgetTypes = new List<EWidgetType>();

        public void DynamicAdd<T>(List<T> L, T _default)
        {
            L.Add(L.Count > 0 ? L[^1] : _default);
        }

        public void Add()
        {
            DynamicAdd(alignments, EWidgetAlignment.Centre);
            DynamicAdd(anchors, EWidgetAnchor.C);
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