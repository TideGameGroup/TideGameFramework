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
        ESINGLE,
        ELIBRARY
    }

    public struct EditorTreeCanvasComponentConstructorArgs
    {
        public UContentManager content;
        public DynamicCanvasComponent dynamicCanvasComponent;
        public TInput input;
        public GameWindow window;
    }

    public class EditorTreeCanvasComponent : UComponent, IUpdateComponent
    {
        private readonly UContentManager content;
        private readonly TInput input;
        private readonly GameWindow window;
        private ETreeCanvasType canvasType;
        private ITreeCanvasFactory factory = null;
        public DynamicCanvasComponent dynamicCanvasComponent;

        public EditorTreeCanvasComponent(EditorTreeCanvasComponentConstructorArgs args)
        {
            TrySetDefault(args.content, out content);
            TrySetDefault(args.dynamicCanvasComponent, out dynamicCanvasComponent);
            TrySetDefault(args.input, out input);
            TrySetDefault(args.window, out window);

            canvasType = ETreeCanvasType.ETREE;
            dynamicCanvasComponent.OnDynamicCanvasSet += () => { RebuildCanvas(); };
            window.ClientSizeChanged += Window_ClientSizeChanged;
        }

        private void Window_ClientSizeChanged(object sender, System.EventArgs e)
        {
            RebuildCanvas();
        }

        public ACanvasComponent CanvasComponent { get; private set; }
        public ACanvasDrawComponent DrawComponent { get; private set; }

        private void RebuildCanvasComponents(FCanvas canvas)
        {
            RemoveChildComponent(CanvasComponent);
            RemoveChildComponent(DrawComponent);

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
                    content = content
                };

            DrawComponent = new ACanvasDrawComponent(canvasRenderArgs);

            AddChildComponent(CanvasComponent);
            AddChildComponent(DrawComponent);
            SetupBindings();
        }

        private void SetupBindings()
        {
            CanvasComponent.BindAction("tree_button.OnPressed", (gt) =>
            {
                canvasType = ETreeCanvasType.ETREE;
                dynamicCanvasComponent.Rebuild();
            });

            CanvasComponent.BindAction("library_button.OnPressed", (gt) =>
            {
                canvasType = ETreeCanvasType.ELIBRARY;
                dynamicCanvasComponent.Rebuild();
            });

            CanvasComponent.BindAction("button_add-1.OnPressed", (gt) =>
            {
                dynamicCanvasComponent.DynamicCanvas.Add("widget1");
                dynamicCanvasComponent.SetSelection(0);
                dynamicCanvasComponent.Rebuild();
            });

            for (int i = 0; i < dynamicCanvasComponent.DynamicCanvas.Count; i++)
            {
                int l = i;
                CanvasComponent.BindAction("button" + i.ToString() + ".OnPressed", (gt) =>
                {
                    dynamicCanvasComponent.SetSelection(l);
                });

                CanvasComponent.BindAction("button_add" + i.ToString() + ".OnPressed", (gt) =>
                {
                    int n = dynamicCanvasComponent.DynamicCanvas.Add(l);
                    dynamicCanvasComponent.SetSelection(n);
                    dynamicCanvasComponent.Rebuild();
                });

                CanvasComponent.BindAction("button_duplicate" + i.ToString() + ".OnPressed", (gt) =>
                {
                    int n = dynamicCanvasComponent.DynamicCanvas.Duplicate(l);
                    dynamicCanvasComponent.SetSelection(n);
                    dynamicCanvasComponent.Rebuild();
                });

                CanvasComponent.BindAction("button_delete" + i.ToString() + ".OnPressed", (gt) =>
                {
                    dynamicCanvasComponent.DynamicCanvas.RemoveAt(l);
                    dynamicCanvasComponent.SetSelection(0);
                    dynamicCanvasComponent.Rebuild();
                });
            }
        }

        public void RebuildCanvas()
        {
            if (dynamicCanvasComponent.DynamicCanvas == null) { return; }

            switch (canvasType)
            {
                case ETreeCanvasType.EAOS:
                    factory = new DynamicAOSFactory(dynamicCanvasComponent.DynamicCanvas);
                    break;

                case ETreeCanvasType.ESINGLE:
                    factory = new DynamicSingleStructFactory(dynamicCanvasComponent.DynamicCanvas, 0);
                    break;

                case ETreeCanvasType.ETREE:
                    factory = new DynamicTreeFactory(dynamicCanvasComponent.DynamicCanvas, window.ClientBounds.Height);
                    break;

                case ETreeCanvasType.ESOA:
                    factory = new DynamicSOAFactory(dynamicCanvasComponent.DynamicCanvas);
                    break;

                case ETreeCanvasType.ELIBRARY:
                    factory = new DynamicLibraryFactory(content, window.ClientBounds.Height);
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