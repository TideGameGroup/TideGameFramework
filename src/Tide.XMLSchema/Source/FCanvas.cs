using Microsoft.Xna.Framework;

namespace Tide.XMLSchema
{
    public enum EWidgetAlignment
    {
        C,
        L,
        R,
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
        PANEL,
        BUTTON,
        LABEL,
        TEXT,
        SLIDER,
        TICKBOX,
        SCROLLBAR,
        TEXTFIELD
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
        public Rectangle root;
        public Rectangle[] sources;
        public string[] texts;
        public string[] textures;
        public string[] tooltips;
        public string[] tooltiptexts;
        public bool[] visibilities;
        public EWidgetType[] widgetTypes;

        public static float ExpectedVersion => 0.1f;

        public static FCanvas Empty(string ID = "", int size = 1)
        {
            FCanvas deepCopy = new FCanvas
            {
                ID = ID,
                anchors = new EWidgetAnchor[size],
                parents = new int[size],
                fonts = new string[size],
                IDs = new string[size],
                textures = new string[size],
                texts = new string[size],
                tooltips = new string[size],
                tooltiptexts = new string[size],
                hoverSounds = new string[size],
                clickSounds = new string[size],
                rectangles = new Rectangle[size],
                root = Rectangle.Empty,
                alignments = new EWidgetAlignment[size],
                sources = new Rectangle[size],
                colors = new Color[size],
                highlightColors = new Color[size],
                visibilities = new bool[size],
                widgetTypes = new EWidgetType[size]
            };
            return deepCopy;
        }

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
                tooltiptexts = new string[tooltiptexts.Length],
                hoverSounds = new string[hoverSounds.Length],
                clickSounds = new string[clickSounds.Length],
                rectangles = new Rectangle[rectangles.Length],
                root = Rectangle.Empty,
                alignments = new EWidgetAlignment[alignments.Length],
                sources = new Rectangle[sources.Length],
                colors = new Color[colors.Length],
                highlightColors = new Color[highlightColors.Length],
                visibilities = new bool[widgetTypes.Length],
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
            tooltiptexts.CopyTo(deepCopy.tooltiptexts, 0);
            rectangles.CopyTo(deepCopy.rectangles, 0);
            deepCopy.root = root;
            alignments.CopyTo(deepCopy.alignments, 0);
            sources.CopyTo(deepCopy.sources, 0);
            colors.CopyTo(deepCopy.colors, 0);
            highlightColors.CopyTo(deepCopy.highlightColors, 0);
            visibilities.CopyTo(deepCopy.visibilities, 0);
            widgetTypes.CopyTo(deepCopy.widgetTypes, 0);

            return deepCopy;
        }
    }
}