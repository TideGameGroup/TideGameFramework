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
        scrollbar
    }

    public struct FCanvas : ISerialisedInstanceData
    {
        public string               ID;
        public Rectangle            root;
        public EWidgetAnchor        anchor;
        public int[]                parents;
        public string[]             fonts;
        public string[]             IDs;
        public string[]             textures;
        public string[]             texts;
        public string[]             tooltips;
        public Rectangle[]          rectangles;
        public EWidgetAlignment[]   alignments;
        public Rectangle[]          sources;
        public Color[]              colors;
        public Color[]              highlightColors;
        public EWidgetType[]        widgetTypes;

        public string Type => "Tide.ACanvas";

        public FCanvas DeepCopy()
        {
            FCanvas deepCopy = new FCanvas();

            deepCopy.ID = ID;
            deepCopy.root = root;
            deepCopy.anchor = anchor;

            deepCopy.parents = new int[parents.Length];
            deepCopy.fonts = new string[fonts.Length];
            deepCopy.IDs = new string[IDs.Length];
            deepCopy.textures = new string[textures.Length];
            deepCopy.texts = new string[texts.Length];
            deepCopy.tooltips = new string[tooltips.Length];
            deepCopy.rectangles = new Rectangle[rectangles.Length];
            deepCopy.alignments = new EWidgetAlignment[alignments.Length];
            deepCopy.sources = new Rectangle[sources.Length];
            deepCopy.colors = new Color[colors.Length];
            deepCopy.highlightColors = new Color[highlightColors.Length];
            deepCopy.widgetTypes = new EWidgetType[widgetTypes.Length];

            parents.CopyTo(deepCopy.parents, 0);
            fonts.CopyTo(deepCopy.fonts, 0);
            IDs.CopyTo(deepCopy.IDs, 0);
            textures.CopyTo(deepCopy.textures, 0);
            texts.CopyTo(deepCopy.texts, 0);
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
