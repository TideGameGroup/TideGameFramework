using Microsoft.Xna.Framework;
using System.Windows.Forms;
using Tide.Core;
using Tide.Tools;
using Tide.XMLSchema;

namespace Tide.Editor
{
    public struct FEditorInterfaceConstructorArgs
    {
        public UContentManager content;
        public UInput input;
        public GameWindow window;
    }

    public class UEditorInterface : UComponent, IUpdateComponent
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
                    canvas = content.Load<FCanvas>("UI"),
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

            SetupBindings(EditorCanvasComponent);
        }

        public ACanvasComponent ActiveCanvasComponent { get; private set; }
        public ACanvasDrawComponent ActiveDrawComponent { get; private set; }
        public ACanvasComponent EditorCanvasComponent { get; private set; }
        public ACanvasDrawComponent EditorDrawComponent { get; private set; }
        public AInputComponent InputComponent { get; private set; }

        private string OpenFileDialog()
        {
            string filePath = "";
            using OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog.FileName;
            }
            return filePath;
        }

        private void OpenUIFile()
        {
            string filePath = OpenFileDialog();
            if (filePath != "")
            {
                string projectDir = ProjectSourcePath.Path + "Content";

                UImportTools.ImportSerialisedData(projectDir, filePath, out FCanvas canvas);

                FCanvasComponentConstructorArgs canvasArgs =
                new FCanvasComponentConstructorArgs
                {
                    audio = null,
                    canvas = canvas,
                    content = content,
                    focus = EFocus.Game,
                    input = InputComponent,
                    scale = 0.5f,
                    window = null
                };

                ActiveCanvasComponent = new ACanvasComponent(canvasArgs);

                Rectangle bounds = content.GraphicsDevice.Viewport.Bounds;
                ActiveCanvasComponent.cache.canvas.root = new Rectangle(bounds.Width / 2, 32, 0, 0);

                FCanvasDrawComponentConstructorArgs canvasRenderArgs =
                    new FCanvasDrawComponentConstructorArgs
                    {
                        component = ActiveCanvasComponent,
                        content = content,
                        input = InputComponent
                    };

                ActiveDrawComponent = new ACanvasDrawComponent(canvasRenderArgs);
            }
        }

        public void SetupBindings(ACanvasComponent canvas)
        {
            canvas.BindAction("open.OnReleased", (gt) => { OpenUIFile(); });
        }

        public void Update(GameTime gameTime)
        {
            if (ActiveCanvasComponent != null)
            {
                RegisterChildComponent(ActiveCanvasComponent);
                RegisterChildComponent(ActiveDrawComponent);

                ActiveCanvasComponent = null;
            }
        }
    }
}