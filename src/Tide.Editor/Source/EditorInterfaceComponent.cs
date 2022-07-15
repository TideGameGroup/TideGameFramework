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

        private FCanvas? _newcanvas = null;

        private string openFilePath = "";

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

        // panels
        public EditorDynamicCanvasComponent DynamicCanvasComponent { get; private set; }

        public ACanvasComponent EditorCanvasComponent { get; private set; }
        public ACanvasDrawComponent EditorDrawComponent { get; private set; }
        public AInputComponent InputComponent { get; private set; }
        public EditorPreviewCanvasComponent PreviewCanvasComponent { get; private set; }
        public EditorPropertiesCanvasComponent PropertiesCanvasComponent { get; private set; }
        public EditorTreeCanvasComponent TreeCanvasComponent { get; private set; }

        private string OpenFileDialog()
        {
            using OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                return openFileDialog.FileName;
            }
            return "";
        }

        private string OpenSaveDialog()
        {
            using SaveFileDialog openFileDialog = new SaveFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                return openFileDialog.FileName;
            }
            return "";
        }

        private void OpenUIFile()
        {
            string filePath = OpenFileDialog();
            if (filePath != "")
            {
                string projectDir = ProjectSourcePath.Path + "Content";
                UImportTools.ImportSerialisedData(projectDir, filePath, out _newcanvas);
            }
        }

        private void SaveFile()
        {
            if (DynamicCanvasComponent.DynamicCanvas == null) { return; }

            if (openFilePath == "")
            {
                SaveFileAs();
            }
            else
            {
                UExportTools.ExportSerialisedInstanceData(openFilePath, DynamicCanvasComponent.DynamicCanvas.AsCanvas());
            }
        }

        private void SaveFileAs()
        {
            if (DynamicCanvasComponent.DynamicCanvas == null) { return; }

            openFilePath = OpenSaveDialog();
            if (openFilePath != "")
            {
                SaveFile();
            }
        }

        public void SetupBindings(ACanvasComponent component)
        {
            component.BindAction("open.OnReleased", (gt) => { OpenUIFile(); });
            component.BindAction("save.OnReleased", (gt) => { SaveFile(); });
            component.BindAction("saveas.OnReleased", (gt) => { SaveFileAs(); });
            component.BindAction("new.OnReleased", (gt) => { DynamicCanvasComponent.New(); });
            component.BindAction("undo.OnReleased", (gt) => { });
        }

        public void Update(GameTime gameTime)
        {
            if (_newcanvas != null)
            {
                DynamicCanvasComponent.Set(new FDynamicCanvas((FCanvas)_newcanvas));
                _newcanvas = null;
            }
        }
    }
}