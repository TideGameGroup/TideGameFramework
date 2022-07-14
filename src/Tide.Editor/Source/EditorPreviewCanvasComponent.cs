using Microsoft.Xna.Framework;
using Tide.Core;
using Tide.XMLSchema;

namespace Tide.Editor
{
    public struct EditorPreviewCanvasComponentConstructorArgs
    {
        public UContentManager content;
        public AInputComponent input;
        public GameWindow window;
        public EditorDynamicCanvasComponent dynamicCanvasComponent;
    }

    public class EditorPreviewCanvasComponent : UComponent, IUpdateComponent
    {
        private readonly UContentManager content;
        private readonly AInputComponent input;
        private readonly GameWindow window;
        private bool rebuild = false;

        public EditorDynamicCanvasComponent dynamicCanvasComponent;

        public EditorPreviewCanvasComponent(EditorPreviewCanvasComponentConstructorArgs args)
        {
            TrySetDefault(args.content, out content);
            TrySetDefault(args.dynamicCanvasComponent, out dynamicCanvasComponent);
            TrySetDefault(args.input, out input);
            TrySetDefault(args.window, out window);

            dynamicCanvasComponent.OnDynamicCanvasUpdated += () => { CanvasComponent.cache.canvas = dynamicCanvasComponent.DynamicCanvas.AsCanvas(); };
            dynamicCanvasComponent.OnDynamicCanvasSet += () => { RebuildCanvas(); };
            //dynamicCanvasComponent.OnSelectionUpdated += () => { RebuildCanvas(); };
        }

        public ACanvasComponent CanvasComponent { get; private set; }
        public ACanvasDrawComponent DrawComponent { get; private set; }

        private void RebuildCanvasComponents(FCanvas canvas)
        {
            UnregisterChildComponent(CanvasComponent);
            UnregisterChildComponent(DrawComponent);

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

            RegisterChildComponent(CanvasComponent);
            RegisterChildComponent(DrawComponent);
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