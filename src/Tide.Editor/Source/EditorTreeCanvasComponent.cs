using Microsoft.Xna.Framework;
using Tide.Core;
using Tide.XMLSchema;

namespace Tide.Editor
{
    public enum ETreeCanvasType
    {
        ETREE,
        ESOA,
        EAOS,
        ESINGLE
    }

    public struct EditorTreeCanvasComponentConstructorArgs
    {
        public UContentManager content;
        public EditorDynamicCanvasComponent dynamicCanvasComponent;
        public AInputComponent input;
        public GameWindow window;
    }

    public class EditorTreeCanvasComponent : UComponent, IUpdateComponent
    {
        private readonly UContentManager content;
        private readonly AInputComponent input;
        private readonly GameWindow window;
        private ETreeCanvasType canvasType;
        private ITreeCanvasFactory factory = null;
        public EditorDynamicCanvasComponent dynamicCanvasComponent;

        public EditorTreeCanvasComponent(EditorTreeCanvasComponentConstructorArgs args)
        {
            TrySetDefault(args.content, out content);
            TrySetDefault(args.dynamicCanvasComponent, out dynamicCanvasComponent);
            TrySetDefault(args.input, out input);
            TrySetDefault(args.window, out window);

            canvasType = ETreeCanvasType.EAOS;

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
            SetupBindings();
        }

        private void SetupBindings()
        {
            for (int i = 0; i < dynamicCanvasComponent.DynamicCanvas.Count; i++)
            {
                int l = i;
                CanvasComponent.BindAction("button" + i.ToString() + ".OnPressed", (gt) =>
                {
                    dynamicCanvasComponent.SetSelection(l);
                });
            }
        }

        public void RebuildCanvas()
        {
            switch (canvasType)
            {
                case ETreeCanvasType.EAOS:
                    factory = new DynamicAOSFactory(dynamicCanvasComponent.DynamicCanvas);
                    break;

                case ETreeCanvasType.ESINGLE:
                    factory = new DynamicSingleStructFactory(dynamicCanvasComponent.DynamicCanvas, 0);
                    break;

                case ETreeCanvasType.ETREE:
                    break;

                case ETreeCanvasType.ESOA:
                    break;

                default:
                    break;
            }
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