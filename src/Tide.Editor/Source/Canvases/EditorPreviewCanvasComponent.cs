using Microsoft.Xna.Framework;
using System;
using Tide.Core;
using Tide.Tools;
using Tide.XMLSchema;

namespace Tide.Editor
{
    public struct EditorPreviewCanvasComponentConstructorArgs
    {
        public UContentManager content;
        public AInputComponent input;
        public GameWindow window;
        public DynamicCanvasComponent dynamicCanvasComponent;
    }

    public class EditorPreviewCanvasComponent : UComponent, IUpdateComponent
    {
        private readonly UContentManager content;
        private readonly AInputComponent input;
        private readonly GameWindow window;
        private bool rebuild = false;

        public DynamicCanvasComponent dynamicCanvasComponent;

        public EditorPreviewCanvasComponent(EditorPreviewCanvasComponentConstructorArgs args)
        {
            TrySetDefault(args.content, out content);
            TrySetDefault(args.dynamicCanvasComponent, out dynamicCanvasComponent);
            TrySetDefault(args.input, out input);
            TrySetDefault(args.window, out window);

            dynamicCanvasComponent.OnDynamicCanvasUpdated += () =>
            { 
                CanvasComponent.cache.canvas = dynamicCanvasComponent.DynamicCanvas.AsCanvas();
                CanvasComponent.cache.canvas.root = new Rectangle(400, 24, 1280, 720);
            };
            dynamicCanvasComponent.OnDynamicCanvasSet += () => { RebuildCanvas(); };

            FCanvasComponentConstructorArgs zoomArgs =
                new FCanvasComponentConstructorArgs
                {
                    audio = null,
                    canvas = content.Load<FCanvas>("Zoom"),
                    content = content,
                    focus = EFocus.Cinematic | EFocus.GameUI,
                    input = input,
                    scale = 1f,
                    window = window
                };

            ZoomCanvasComponent = new ACanvasComponent(zoomArgs);

            FCanvasDrawComponentConstructorArgs zoomRenderArgs =
                new FCanvasDrawComponentConstructorArgs
                {
                    component = ZoomCanvasComponent,
                    content = content,
                    input = input
                };

            ZoomDrawComponent = new ACanvasDrawComponent(zoomRenderArgs);
            ZoomCanvasComponent.cache.canvas.root = new Rectangle(400, 24, 0, 0);
            ZoomCanvasComponent.BindAction("widthtext.OnTextEntered", (gt) => { ResetPreviewWindowCanvas(); });
            ZoomCanvasComponent.BindAction("heighttext.OnTextEntered", (gt) => { ResetPreviewWindowCanvas(); });

            FCanvasComponentConstructorArgs canvasArgs =
                new FCanvasComponentConstructorArgs
                {
                    audio = null,
                    canvas = GetPreviewWindowCanvas(new Rectangle(0, 0, 1280, 720)),
                    content = content,
                    focus = EFocus.Cinematic | EFocus.GameUI,
                    input = input,
                    scale = 1f,
                    window = window
                };

            PreviewCanvasComponent = new ACanvasComponent(canvasArgs);

            FCanvasDrawComponentConstructorArgs canvasRenderArgs =
                new FCanvasDrawComponentConstructorArgs
                {
                    component = PreviewCanvasComponent,
                    content = content,
                    input = input
                };

            PreviewDrawComponent = new ACanvasDrawComponent(canvasRenderArgs);

            AddChildComponent(ZoomCanvasComponent);
            AddChildComponent(ZoomDrawComponent);
            AddChildComponent(PreviewCanvasComponent);
            AddChildComponent(PreviewDrawComponent);

            window.ClientSizeChanged += Window_ClientSizeChanged;
        }

        private void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            ResetPreviewWindowCanvas();
        }

        private bool GetAbsolutePreviewBounds(out Rectangle rect)
        {
            string wstr = ZoomCanvasComponent.cache.canvas.texts[ZoomCanvasComponent.graph.widgetNameIndexMap["widthtext"]];
            string hstr = ZoomCanvasComponent.cache.canvas.texts[ZoomCanvasComponent.graph.widgetNameIndexMap["heighttext"]];

            if (int.TryParse(wstr, out int w) && int.TryParse(hstr, out int h))
            {
                Rectangle view = new Rectangle(400, 24, window.ClientBounds.Width - 400, window.ClientBounds.Height - 24);
                rect = new Rectangle(view.Center.X - (w / 2), view.Center.Y - (h / 2), w, h);
                return true;
            }
            rect = default;
            return false;
        }

        private FCanvas GetPreviewWindowCanvas(Rectangle bounds)
        {
            FDynamicCanvas newCanvas = new FDynamicCanvas("preview");
            Rectangle view = window.ClientBounds;
            newCanvas.root = new Rectangle(400, 24, view.Width - 400, view.Height - 24);

            newCanvas.Add(
                    "panel",
                    parent: -1,
                    anchor: EWidgetAnchor.C,
                    rectangle: new Rectangle(-bounds.Width / 2, -bounds.Height / 2, bounds.Width, bounds.Height),
                    source: new Rectangle(240, 0, 16, 16),
                    texture: "Icons",
                    color: Color.LightGray,
                    highlightColor: Color.LightGray,
                    widgetType: EWidgetType.PANEL
                    );

            return newCanvas.AsCanvas();
        }

        private void ResetPreviewWindowCanvas()
        {
            string wstr = ZoomCanvasComponent.cache.canvas.texts[ZoomCanvasComponent.graph.widgetNameIndexMap["widthtext"]];
            string hstr = ZoomCanvasComponent.cache.canvas.texts[ZoomCanvasComponent.graph.widgetNameIndexMap["heighttext"]];

            if (int.TryParse(wstr, out int w) && int.TryParse(hstr, out int h))
            {
                Rectangle bounds = new Rectangle(0, 0, w, h);

                PreviewCanvasComponent.cache.canvas = GetPreviewWindowCanvas(bounds);
            }
        }

        public ACanvasComponent ZoomCanvasComponent { get; private set; }
        public ACanvasDrawComponent ZoomDrawComponent { get; private set; }
        public ACanvasComponent PreviewCanvasComponent { get; private set; }
        public ACanvasDrawComponent PreviewDrawComponent { get; private set; }
        public ACanvasComponent CanvasComponent { get; private set; }
        public ACanvasDrawComponent DrawComponent { get; private set; }

        private void RebuildCanvasComponents(FCanvas canvas)
        {
            RemoveChildComponent(CanvasComponent);
            RemoveChildComponent(DrawComponent);

            canvas.root = new Rectangle(400, 24, 1280, 720);
            if (GetAbsolutePreviewBounds(out Rectangle rect))
            {
                canvas.root = rect;
            }

            FCanvasComponentConstructorArgs canvasArgs =
                new FCanvasComponentConstructorArgs
                {
                    audio = null,
                    canvas = canvas,
                    content = content,
                    focus = EFocus.Cinematic | EFocus.GameUI,
                    input = input,
                    scale = 1f,
                    window = window
                };

            CanvasComponent = new ACanvasComponent(canvasArgs);

            FCanvasDrawComponentConstructorArgs canvasRenderArgs =
                new FCanvasDrawComponentConstructorArgs
                {
                    component = CanvasComponent,
                    content = content,
                    input = input
                };

            DrawComponent = new ACanvasDrawComponent(canvasRenderArgs);

            AddChildComponent(CanvasComponent);
            AddChildComponent(DrawComponent);
        }

        public void RebuildCanvas()
        {
            rebuild = true;
        }

        public void Update(GameTime gameTime)
        {
            if (rebuild)
            {
                RebuildCanvasComponents(dynamicCanvasComponent.DynamicCanvas.AsCanvas());
                rebuild = false;
            }
        }
    }
}