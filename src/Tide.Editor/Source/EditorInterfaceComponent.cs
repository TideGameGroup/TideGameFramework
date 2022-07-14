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

            // top bar
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

            // top bar draw
            FCanvasDrawComponentConstructorArgs canvasRenderArgs =
                new FCanvasDrawComponentConstructorArgs
                {
                    component = EditorCanvasComponent,
                    content = content,
                    input = InputComponent
                };

            EditorDrawComponent = new ACanvasDrawComponent(canvasRenderArgs);

            // dynamic canvas

            DynamicCanvasComponent = new EditorDynamicCanvasComponent();

            // tree canvas
            EditorTreeCanvasComponentConstructorArgs treeArgs =
                new EditorTreeCanvasComponentConstructorArgs
                {
                    content = content,
                    input = InputComponent,
                    window = args.window,
                    dynamicCanvasComponent = DynamicCanvasComponent
                };

            TreeCanvasComponent = new EditorTreeCanvasComponent(treeArgs);

            // property canvas
            EditorPropertiesCanvasComponentConstructorArgs propertiesArgs =
                new EditorPropertiesCanvasComponentConstructorArgs
                {
                    content = content,
                    input = InputComponent,
                    window = args.window,
                    dynamicCanvasComponent = DynamicCanvasComponent
                };

            PropertiesCanvasComponent = new EditorPropertiesCanvasComponent(propertiesArgs);

            // preview canvas
            EditorPreviewCanvasComponentConstructorArgs previewArgs =
                new EditorPreviewCanvasComponentConstructorArgs
                {
                    content = content,
                    input = InputComponent,
                    window = args.window,
                    dynamicCanvasComponent = DynamicCanvasComponent
                };

            PreviewCanvasComponent = new EditorPreviewCanvasComponent(previewArgs);

            RegisterChildComponent(InputComponent);
            RegisterChildComponent(EditorCanvasComponent);
            RegisterChildComponent(EditorDrawComponent);
            RegisterChildComponent(DynamicCanvasComponent);
            RegisterChildComponent(PreviewCanvasComponent);
            RegisterChildComponent(TreeCanvasComponent);
            RegisterChildComponent(PropertiesCanvasComponent);

            SetupBindings(EditorCanvasComponent);
        }

        public ACanvasComponent EditorCanvasComponent { get; private set; }
        public ACanvasDrawComponent EditorDrawComponent { get; private set; }
        public AInputComponent InputComponent { get; private set; }

        // panels
        public EditorDynamicCanvasComponent DynamicCanvasComponent { get; private set; }
        public EditorTreeCanvasComponent TreeCanvasComponent { get; private set; }
        public EditorPropertiesCanvasComponent PropertiesCanvasComponent { get; private set; }
        public EditorPreviewCanvasComponent PreviewCanvasComponent { get; private set; }

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

        private FCanvas? _newcanvas = null;

        private void OpenUIFile()
        {
            string filePath = OpenFileDialog();
            if (filePath != "")
            {
                string projectDir = ProjectSourcePath.Path + "Content";
                UImportTools.ImportSerialisedData(projectDir, filePath, out _newcanvas);
            }
        }

        public void SetupBindings(ACanvasComponent canvas)
        {
            canvas.BindAction("open.OnReleased", (gt) => { OpenUIFile(); });
        }

        public void Update(GameTime gameTime)
        {
            if (_newcanvas != null)
            {
                InputComponent.ClearBindings();
                DynamicCanvasComponent.Set(new FDynamicCanvas((FCanvas)_newcanvas));
                SetupBindings(EditorCanvasComponent);
                _newcanvas = null;
            }
        }
    }
}