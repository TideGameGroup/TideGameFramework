using Microsoft.Xna.Framework;
using Tide.Core;
using Tide.Tools;
using Tide.XMLSchema;

namespace Tide.Editor
{
    public struct EditorPropertiesCanvasComponentConstructorArgs
    {
        public UContentManager content;
        public EditorDynamicCanvasComponent dynamicCanvasComponent;
        public AInputComponent input;
        public GameWindow window;
    }

    public class EditorPropertiesCanvasComponent : UComponent, IUpdateComponent
    {
        private readonly UContentManager content;
        private readonly AInputComponent input;
        private readonly GameWindow window;
        private ITreeCanvasFactory factory = null;
        public EditorDynamicCanvasComponent dynamicCanvasComponent;

        public EditorPropertiesCanvasComponent(EditorPropertiesCanvasComponentConstructorArgs args)
        {
            TrySetDefault(args.content, out content);
            TrySetDefault(args.input, out input);
            TrySetDefault(args.window, out window);
            TrySetDefault(args.dynamicCanvasComponent, out dynamicCanvasComponent);

            //dynamicCanvasComponent.OnDynamicCanvasUpdated += () => { CanvasComponent.cache.canvas = dynamicCanvasComponent.DynamicCanvas.AsCanvas(); };
            dynamicCanvasComponent.OnDynamicCanvasSet += () => { RebuildCanvas(); };
            dynamicCanvasComponent.OnSelectionUpdated += () => { RebuildCanvas(); };
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

            SetupBindings(CanvasComponent);
        }

        private void SetupBindings(ACanvasComponent canvas)
        {
            canvas.BindAction("ID_field.OnTextEntered", (gt) => {
                dynamicCanvasComponent.DynamicCanvas.IDs[dynamicCanvasComponent.selection] =
                CanvasComponent.cache.canvas.texts[CanvasComponent.graph.widgetNameIndexMap["ID_field"]];
                dynamicCanvasComponent.Rebuild();
            });

            canvas.BindAction("text_field.OnTextEntered", (gt) => {
                dynamicCanvasComponent.DynamicCanvas.texts[dynamicCanvasComponent.selection] =
                CanvasComponent.cache.canvas.texts[CanvasComponent.graph.widgetNameIndexMap["text_field"]];
                dynamicCanvasComponent.Refresh();
            });

        }

        public void RebuildCanvas()
        {
            factory = new DynamicSingleStructFactory(dynamicCanvasComponent.DynamicCanvas, dynamicCanvasComponent.selection);
        }

        public void Update(GameTime gameTime)
        {
            if (factory != null)
            {
                RebuildCanvasComponents(factory.GetCanvas());
                factory = null;
            }
        }
    }
}