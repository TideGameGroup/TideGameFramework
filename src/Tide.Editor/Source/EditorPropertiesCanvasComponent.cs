using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Tide.Core;
using Tide.Tools;
using Tide.XMLSchema;

namespace Tide.Editor
{

    public struct EditorPropertiesCanvasComponentConstructorArgs
    {
        public UContentManager content;
        public UInput input;
        public GameWindow window;
        public EditorTreeCanvasComponent treeCanvasComponent;
    }

    public class EditorPropertiesCanvasComponent : UComponent, IUpdateComponent
    {
        private readonly UContentManager content;
        private readonly GameWindow window;
        private readonly EditorTreeCanvasComponent treeCanvasComponent;
        private ITreeCanvasFactory factory = null;

        public EditorPropertiesCanvasComponent(EditorPropertiesCanvasComponentConstructorArgs args)
        {
            NullCheck(args.input);
            TrySetDefault(args.content, out content);
            TrySetDefault(args.window, out window);
            TrySetDefault(args.treeCanvasComponent, out treeCanvasComponent);

            InputComponent = new AInputComponent(args.input);
            RegisterChildComponent(InputComponent);
        }

        public FDynamicCanvas DynamicCanvas => treeCanvasComponent.DynamicCanvas;
        public ACanvasComponent CanvasComponent { get; private set; }
        public ACanvasDrawComponent DrawComponent { get; private set; }
        public AInputComponent InputComponent { get; private set; }

        public void GenerateTree()
        {
            GenerateTree(DynamicCanvas);
        }

        public void GenerateTree(FCanvas canvas)
        {
            GenerateTree(new FDynamicCanvas(canvas));
        }

        public void GenerateTree(FDynamicCanvas dynamicCanvas)
        {
            factory = new DynamicSingleStructFactory(dynamicCanvas, 0);
        }

        public void RebuildTree(FCanvas canvas)
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
                    input = InputComponent,
                    scale = 1f,
                    window = window
                };

            CanvasComponent = new ACanvasComponent(canvasArgs);

            FCanvasDrawComponentConstructorArgs canvasRenderArgs =
                new FCanvasDrawComponentConstructorArgs
                {
                    component = CanvasComponent,
                    content = content,
                    input = InputComponent
                };

            DrawComponent = new ACanvasDrawComponent(canvasRenderArgs);

            RegisterChildComponent(CanvasComponent);
            RegisterChildComponent(DrawComponent);
        }

        public void Update(GameTime gameTime)
        {
            if (factory != null)
            {
                RebuildTree(factory.GetCanvas());
                factory = null;
            }
        }
    }
}
