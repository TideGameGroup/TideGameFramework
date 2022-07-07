using Microsoft.Xna.Framework;

namespace Tide.XMLSchema
{
    public enum EWidgetAlignment
    {
        Centre,
        Left,
        Right
    }

    public enum EWidgetAnchor
    {
        N,
        NE,
        E,
        SE,
        S,
        SW,
        W,
        NW,
        C
    }

    public enum EWidgetType
    {
        panel,
        button,
        label,
        text,
        slider,
        tickbox,
        scrollbar,
        textfield
    }

    public struct FCanvas : ISerialisedInstanceData
    {
        public EWidgetAlignment[] alignments;
        public EWidgetAnchor[] anchors;
        public string[] clickSounds;
        public Color[] colors;
        public string[] fonts;
        public Color[] highlightColors;
        public string[] hoverSounds;
        public string ID;
        public string[] IDs;
        public int[] parents;
        public Rectangle[] rectangles;
        public Rectangle[] sources;
        public string[] texts;
        public string[] textures;
        public string[] tooltips;
        public EWidgetType[] widgetTypes;

        public string Type => "Tide.ACanvas";

        public FCanvas DeepCopy()
        {
            FCanvas deepCopy = new FCanvas
            {
                ID = ID,
                anchors = new EWidgetAnchor[anchors.Length],
                parents = new int[parents.Length],
                fonts = new string[fonts.Length],
                IDs = new string[IDs.Length],
                textures = new string[textures.Length],
                texts = new string[texts.Length],
                tooltips = new string[tooltips.Length],
                hoverSounds = new string[tooltips.Length],
                clickSounds = new string[tooltips.Length],
                rectangles = new Rectangle[rectangles.Length],
                alignments = new EWidgetAlignment[alignments.Length],
                sources = new Rectangle[sources.Length],
                colors = new Color[colors.Length],
                highlightColors = new Color[highlightColors.Length],
                widgetTypes = new EWidgetType[widgetTypes.Length]
            };

            anchors.CopyTo(deepCopy.anchors, 0);
            parents.CopyTo(deepCopy.parents, 0);
            fonts.CopyTo(deepCopy.fonts, 0);
            IDs.CopyTo(deepCopy.IDs, 0);
            textures.CopyTo(deepCopy.textures, 0);
            texts.CopyTo(deepCopy.texts, 0);
            hoverSounds.CopyTo(deepCopy.hoverSounds, 0);
            clickSounds.CopyTo(deepCopy.clickSounds, 0);
            tooltips.CopyTo(deepCopy.tooltips, 0);
            rectangles.CopyTo(deepCopy.rectangles, 0);
            alignments.CopyTo(deepCopy.alignments, 0);
            sources.CopyTo(deepCopy.sources, 0);
            colors.CopyTo(deepCopy.colors, 0);
            highlightColors.CopyTo(deepCopy.highlightColors, 0);
            widgetTypes.CopyTo(deepCopy.widgetTypes, 0);

            return deepCopy;
        }
    }
}