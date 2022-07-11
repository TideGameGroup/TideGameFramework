using Microsoft.Xna.Framework;
using System;
using Tide.Core;
using Tide.Tools;
using Tide.XMLSchema;
using System.Windows.Forms;

namespace Tide.Editor
{
    public struct FEditorInterfaceConstructorArgs
    {
        public UInput input;
        public UContentManager content;
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
                    canvas = GenerateBaseCanvas(),
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

        public AInputComponent InputComponent { get; private set; }
        public ACanvasComponent EditorCanvasComponent { get; private set; }
        public ACanvasDrawComponent EditorDrawComponent { get; private set; }
        public ACanvasComponent ActiveCanvasComponent { get; private set; }
        public ACanvasDrawComponent ActiveDrawComponent { get; private set; }

        public static FCanvas GenerateBaseCanvas()
        {
            FCanvas canvas = new FCanvas
            {
                ID = "UExportToolsCanvas",
                anchors = new EWidgetAnchor[1] { EWidgetAnchor.C },
                clickSounds = new string[1] { "" },
                parents = new int[1] { -1 },
                fonts = new string[1] { "Arial" },
                IDs = new string[1] { "open" },
                hoverSounds = new string[1] { "" },
                textures = new string[1] { "" },
                texts = new string[1] { "textfield" },
                tooltips = new string[1] { "" },
                rectangles = new Rectangle[1] { new Rectangle(-100, -30, 100, 30) },
                alignments = new EWidgetAlignment[1] { EWidgetAlignment.Left },
                sources = new Rectangle[1] { new Rectangle(0, 0, 1, 1) },
                colors = new Color[1] { Color.Gray },
                highlightColors = new Color[1] { Color.White },
                widgetTypes = new EWidgetType[1] { EWidgetType.button },
            };

            return canvas;
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
                    scale = 1f,
                    window = null
                };

                ActiveCanvasComponent = new ACanvasComponent(canvasArgs);

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
    }
}
