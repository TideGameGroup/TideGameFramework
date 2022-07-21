using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Tide.XMLSchema;

namespace Tide.Core
{
    public struct FCanvasGraphConstructorArgs
    {
        public UContentManager content;
        public FCanvasCache cache;
        public float scale;
    }

    public class FCanvasGraph
    {
        private readonly FCanvasCache cache;
        private readonly UContentManager content;

        public readonly Dictionary<string, int> widgetNameIndexMap = new Dictionary<string, int>();

        public FCanvasGraph(FCanvasGraphConstructorArgs args)
        {
            cache = args.cache;
            content = args.content;
            Scale = args.scale;

            for (int i = 0; i < cache.canvas.IDs.Length; i++)
            {
                widgetNameIndexMap.Add(cache.canvas.IDs[i], i);
            }
        }

        public float Scale { get; set; }
        public float Count => cache.canvas.parents.Length;

        public static Rectangle GetAnchoredRectangle(Rectangle rect, Rectangle root, EWidgetAnchor anchor)
        {
            switch (anchor)
            {
                case EWidgetAnchor.N:
                    rect.X += (root.Width / 2);
                    break;

                case EWidgetAnchor.NE:
                    rect.X += root.Width;
                    break;

                case EWidgetAnchor.E:
                    rect.X += root.Width;
                    rect.Y += (root.Height / 2);
                    break;

                case EWidgetAnchor.SE:
                    rect.X += root.Width;
                    rect.Y += root.Height;
                    break;

                case EWidgetAnchor.S:
                    rect.X += (root.Width / 2);
                    rect.Y += root.Height;
                    break;

                case EWidgetAnchor.SW:
                    rect.Y += root.Height;
                    break;

                case EWidgetAnchor.W:
                    rect.Y += root.Height / 2;
                    break;

                case EWidgetAnchor.NW:
                    break;

                case EWidgetAnchor.C:
                    rect.X += (root.Width / 2);
                    rect.Y += (root.Height / 2);
                    break;

                default:
                    break;
            }

            rect.Offset(root.Location);
            return rect;
        }

        public Rectangle GetLocalRectangle(FCanvas canvas, int i)
        {
            Rectangle rect = canvas.rectangles[i];

            rect.X = (int)(rect.X * Scale);
            rect.Y = (int)(rect.Y * Scale);
            rect.Width = (int)(rect.Width * Scale);
            rect.Height = (int)(rect.Height * Scale);

            float width = rect.Width;

            if (canvas.widgetTypes[i] == EWidgetType.TEXT)
            {
                width = cache.fontCache[canvas.fonts[i]].MeasureString(canvas.texts[i]).X;
            }

            switch (canvas.alignments[i])
            {
                case EWidgetAlignment.C:
                    rect.Offset(new Vector2(-width / 2, 0.0f));
                    break;

                case EWidgetAlignment.R:
                    rect.Offset(new Vector2(-width, 0.0f));
                    break;

                default:
                    break;
            }

            return rect;
        }

        public Rectangle GetMinRectangle(Rectangle A, Rectangle B)
        {
            return new Rectangle
            {
                X = A.X,
                Y = A.Y,
                Width = B.Width == 0 ? A.Width : Math.Min(A.Width, B.Width),
                Height = B.Height == 0 ? A.Height : Math.Min(A.Height, B.Height)
            };
        }

        public Rectangle GetRectangle(FCanvas canvas, int i)
        {
            if (i == -1)
            {
                Rectangle rect = GetMinRectangle(content.GraphicsDevice.Viewport.Bounds, canvas.root);
                rect.Offset(canvas.root.Location);
                return rect;
            }

            Rectangle parentRect = GetRectangle(canvas, canvas.parents[i]);
            return GetRectangleInParent(canvas, i, parentRect);
        }

        public Rectangle GetRectangleInParent(FCanvas canvas, int i, Rectangle parentRect)
        {
            Rectangle rect = GetLocalRectangle(canvas, i);
            return GetAnchoredRectangle(rect, parentRect, canvas.anchors[i]);
        }
    }
}