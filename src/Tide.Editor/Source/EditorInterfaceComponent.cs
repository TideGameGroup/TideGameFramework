using Microsoft.Xna.Framework;
using Tide.Core;
using Tide.Tools;
using Tide.XMLSchema;

namespace Tide.Editor
{
    public struct FEditorInterfaceConstructorArgs
    {
        public UInput input;
        public UContentManager content;
        public GameWindow window;
    }

    public class UEditorInterface : UComponent
    {
        private readonly UContentManager content;

        public UEditorInterface(FEditorInterfaceConstructorArgs args)
        {
            NullCheck(args.input);
            TrySetDefault(args.content, out content);

            InputComponent = new AInputComponent(args.input);

            FCanvasComponentConstructorArgs canvasArgs =
                new FCanvasComponentConstructorArgs
                {
                    audio = null,
                    canvas = UExportTools.GenerateBlankCanvas(),
                    content = content,
                    focus = EFocus.Cinematic | EFocus.GameUI,
                    input = InputComponent,
                    scale = 1f,
                    window = args.window
                };

            EditorCanvasComponent = new ACanvasComponent(canvasArgs);

            FCanvasDrawComponentConstructorArgs canvasRenderArgs =
                new FCanvasDrawComponentConstructorArgs
                {
                    component = EditorCanvasComponent,
                    content = content,
                    input = InputComponent
                };

            EditorDrawComponent = new ACanvasDrawComponent(canvasRenderArgs);

            RegisterChildComponent(InputComponent);
            RegisterChildComponent(EditorCanvasComponent);
            RegisterChildComponent(EditorDrawComponent);
        }

        public AInputComponent InputComponent { get; private set; }
        public ACanvasComponent EditorCanvasComponent { get; private set; }
        public ACanvasDrawComponent EditorDrawComponent { get; private set; }
        public ACanvasComponent ActiveCanvasComponent { get; private set; }
        public ACanvasDrawComponent ActiveDrawComponent { get; private set; }
    }
}
