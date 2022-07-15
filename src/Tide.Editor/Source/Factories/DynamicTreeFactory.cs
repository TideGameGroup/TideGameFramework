using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Tide.Tools;
using Tide.XMLSchema;

namespace Tide.Editor
{
    public struct Treenode
    {
        public List<Treenode> children;
        public int depth;
        public int index;

        public Treenode(int index, int depth)
        {
            this.index = index;
            this.depth = depth;
            children = new List<Treenode>();
        }
    }

    public class DynamicTreeFactory : ITreeCanvasFactory
    {
        private readonly FDynamicCanvas canvas = null;
        private FDynamicCanvas newCanvas = null;

        public DynamicTreeFactory(FDynamicCanvas canvas, int height)
        {
            this.canvas = canvas;

            newCanvas = new FDynamicCanvas("Tree");
            newCanvas.root = new Rectangle(0, 24, 0, 0);

            ITreeCanvasFactory.AddTreePanel(newCanvas, height);

            int place = 0;
            DrawTree(canvas, NewNode(canvas, -1, 0), ref place);
        }

        private void DrawTree(FDynamicCanvas canvas, Treenode tree, ref int p)
        {
            int depthoffset = tree.depth * 16;
            int placeoffset = p * 20;

            newCanvas.Add(
                "panel" + tree.index.ToString(),
                widgetType: EWidgetType.PANEL,
                rectangle: new Rectangle(24 + depthoffset, 24 + placeoffset, 16, 16),
                source: new Rectangle(80, 48, 16, 16),
                texture: "Icons",
                color: Color.White,
                highlightColor: Color.White
            );

            newCanvas.Add(
                "button_add" + tree.index.ToString(),
                rectangle: new Rectangle(160 + depthoffset, 24 + placeoffset, 16, 16),
                source: new Rectangle(80, 16, 16, 16),
                texture: "Icons",
                color: Color.LightGray,
                highlightColor: Color.White
            );

            if (tree.index != -1)
            {
                newCanvas.Add(
                    "button" + tree.index.ToString(),
                    rectangle: new Rectangle(40 + depthoffset, 24 + placeoffset, 100, 16),
                    source: new Rectangle(240, 0, 16, 16),
                    texture: "Icons",
                    color: Color.LightGray,
                    highlightColor: Color.White
                );
                
                newCanvas.Add(
                    "button_duplicate" + tree.index.ToString(),
                    rectangle: new Rectangle(142 + depthoffset, 24 + placeoffset, 16, 16),
                    source: new Rectangle(128, 0, 16, 16),
                    texture: "Icons",
                    color: Color.LightGray,
                    highlightColor: Color.White
                );

                if (tree.children.Count == 0)
                {
                    newCanvas.Add(
                        "button_delete" + tree.index.ToString(),
                        rectangle: new Rectangle(380, 24 + placeoffset, 16, 16),
                        source: new Rectangle(64, 0, 16, 16),
                        texture: "Icons",
                        color: Color.LightGray,
                        highlightColor: Color.White
                    );
                }

                newCanvas.Add(
                   "text" + tree.index.ToString(),
                    widgetType: EWidgetType.TEXT,
                    alignment: EWidgetAlignment.L,
                    rectangle: new Rectangle(40 + depthoffset, 24 + placeoffset, 100, 16),
                    source: new Rectangle(0, 0, 0, 0),
                    text: canvas.IDs[tree.index],
                    font: "consolas",
                    color: Color.DarkSlateGray,
                    highlightColor: Color.DarkSlateGray
                 );
            }

            foreach (var child in tree.children)
            {
                p++;
                DrawTree(canvas, child, ref p);
            }
        }

        private Treenode NewNode(FDynamicCanvas canvas, int index, int depth)
        {
            Treenode node = new Treenode(index, depth);
            for (int i = 0; i < canvas.parents.Count; i++)
            {
                if (canvas.parents[i] == index)
                {
                    node.children.Add(NewNode(canvas, i, depth + 1));
                }
            }
            return node;
        }

        public FCanvas GetCanvas()
        {
            return newCanvas.AsCanvas();
        }

        public FDynamicCanvas GetDynamicCanvas()
        {
            return newCanvas;
        }
    }
}